namespace Sales.ViewModels
{
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Helpers;
    using Sales.Services;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class EditProductViewModel : BaseViewModel
    {
        #region Attrubutes
        private Product product;

        private MediaFile file;

        private ImageSource imageSource;

        private APIService apiService;

        private bool isRunning;

        private bool isEnabled;

        #endregion

        #region Properties
        public Product Product
        {
            get { return this.product; }
            set { this.SetValue(ref this.product, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.SetValue(ref this.isEnabled, value); }
        }
        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }
        #endregion

        #region Contructors
        public EditProductViewModel(Product product)
        {
            this.product = product;
            this.apiService = new APIService();
            this.IsEnabled = true;
            this.ImageSource = product.ImageFullPath;
        }
        #endregion

        #region Commands

        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }

        private async void Delete()
        {
            var answer = await Application.Current.MainPage.DisplayAlert(
                 Languages.Confirm,
                 Languages.DeleteConfirmation,
                 Languages.Yes,
                 Languages.No);
            if (!answer)
            {
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Delete(url, prefix, controller, this.Product.Productid);

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.MyProducts.Where(p => p.Productid == this.Product.Productid).FirstOrDefault();
            if (deleteProduct != null)
            {
                productsViewModel.MyProducts.Remove(deleteProduct);
            }

            productsViewModel.RefreshList();

            this.IsRunning = false;
            this.IsEnabled = true;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ImageSource,
                 Languages.Cancel,
                null,
                Languages.FromGallery,
                Languages.NewPicture);
            if (source == Languages.Cancel)
            {
                this.file = null;
                return;
            }
            if (source == Languages.NewPicture)
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Small,
                }
                );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }
            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = this.file.GetStream();
                    return stream;
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        public string[] FromGallery { get; private set; }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Product.Description))
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.DescriptionError, Languages.Accept);
                return;
            }
  
            if (this.Product.Price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.PriceError, Languages.Accept);
                return;
            }
            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
                this.Product.ImageArray = imageArray;
            }
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Put(url, prefix, controller, this.Product, this.Product.Productid );

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var newProduct = (Product)response.Result;
            var productsViewModel = ProductsViewModel.GetInstance();
            var OldProduct = productsViewModel.MyProducts.Where(p => p.Productid == this.Product.Productid).FirstOrDefault();

            if (OldProduct != null)
            {
                productsViewModel.MyProducts.Remove(OldProduct);
            }
            productsViewModel.MyProducts.Add(newProduct);
            productsViewModel.RefreshList();
            this.IsRunning = false;
            this.IsEnabled = true;
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        #endregion
    }
}
