namespace Sales.ViewModels
{
    using System.Linq;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Sales.Views;
    using Services;
    using Xamarin.Forms;

    public class ProductItemViewModel : Product
    {

        #region attributes
        private APIService apiService;
        #endregion

        #region Contructors
        public ProductItemViewModel ()
        {
            this.apiService = new APIService();
        }
        #endregion
        #region Commands

        public ICommand EditProductCommand
        {
            get
            {
                return new RelayCommand(EditProduct);
            }
        }

        private async void  EditProduct()
        {
            MainViewModel .GetInstance().EditProduct = new EditProductViewModel(this);
            await Application.Current.MainPage.Navigation.PushAsync(new EditProductPage());
        }

        public ICommand DeleteProductCommand 
        {
            get
            {
                return new RelayCommand(DeleteProduct);
            }
        }

        private async void DeleteProduct()
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

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Delete(url, prefix, controller, this.Productid );

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.MyProducts.Where(p => p.Productid == this.Productid).FirstOrDefault();
            if (deleteProduct != null)
            {
                productsViewModel.MyProducts.Remove(deleteProduct);
            }

            productsViewModel.RefreshList();
        }

        #endregion
    }
}
        