namespace Sales.ViewModels
{
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Sales.Helpers;
    using Services;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel 
    {
        #region Attributes

        private APIService apiService;

        private bool isRefreshing;

        private ObservableCollection<ProductItemViewModel> products;
        #endregion

        #region Properties

        public List<Product> MyProducts { get; set; }
        public ObservableCollection<ProductItemViewModel> Products 
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value);}
        }
        public bool IsRefreshing
                {
                    get { return this.isRefreshing; }
                    set { this.SetValue(ref this.isRefreshing, value); }
                }
        #endregion

        #region Contructors
         public ProductsViewModel()
                {
                    instance = this;
                    this.apiService = new APIService();
                    this.LoadProducts();
                }
        #endregion

        #region Singleton
        private static ProductsViewModel instance;

        public static ProductsViewModel GetInstance()
        {
            if(instance == null)
            {
                return new ProductsViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        private async void LoadProducts()
        {
            this.IsRefreshing = true;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.isRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.GetList<Product>(url, prefix, controller);

            if (!response.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            this.MyProducts = (List<Product>)response.Result;
            this.RefreshList();
            this.IsRefreshing = false;
        }

        public void RefreshList()
        {
            var mylistProductItemViewModel = MyProducts.Select(p => new ProductItemViewModel
            {
                Description = p.Description,
                ImageArray = p.ImageArray,
                ImagePath = p.ImagePath,
                IsAvailable = p.IsAvailable,
                Price = p.Price,
                Productid = p.Productid,
                PublishOn = p.PublishOn,
                Remarks = p.Remarks,
            });
            this.Products = new ObservableCollection<ProductItemViewModel>(
                mylistProductItemViewModel.OrderBy (p => p.Description ));
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }
        } 
        #endregion
    }
}
