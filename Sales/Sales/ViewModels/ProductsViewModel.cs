namespace Sales.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class ProductsViewModel : BaseViewModel 
    {
        #region Attributes

        private string filter;

        private APIService apiService;

        private DataService dataService;

        private bool isRefreshing;

        private ObservableCollection<ProductItemViewModel> products;
        #endregion

        #region Properties

        public string Filter 
        {
            get { return this.filter; }
            set 
            { 
                this.filter = value;
                this.RefreshList();
            }
        }


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
                    this.dataService = new DataService();
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
            if (connection.IsSuccess)
            {
                var answer = await this.LoadProductsFromAPI();
                if (answer)
                {
                    this.SaveProductsToDB();
                }
               
            }

            else
            {
                await this.LoadProductsFromDB();
            }
                
                if (this.MyProducts == null || this.MyProducts.Count == 0)
            {
                this.isRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.NoProductsMessage, Languages.Accept);
                return;
            }
           
            this.RefreshList();
            this.IsRefreshing = false;
        }

        private async Task LoadProductsFromDB()
        {
            this.MyProducts = await this.dataService.GetAllProducts();
        }

        private async Task SaveProductsToDB()
        {
            await dataService.DeleteAllProducts();
            this.dataService.Insert(this.MyProducts);
        }

        private async Task<bool> LoadProductsFromAPI()
        {
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.GetList<Product>(url, prefix, controller, Settings.TokenType, Settings.AccessToken);

            if (!response.IsSuccess)
            {
                return false;
            }

            this.MyProducts = (List<Product>)response.Result;
            return true;
        }

        public void RefreshList()
        {
            if (string.IsNullOrEmpty(this.Filter))
            {
                var mylistProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
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
           else
            {
                var mylistProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    Productid = p.Productid,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,
                }).Where(p => p.Description.ToLower().Contains(this.Filter.ToLower())).ToList();
                this.Products = new ObservableCollection<ProductItemViewModel>(
                    mylistProductItemViewModel.OrderBy(p => p.Description));
            }
        }
        #endregion

        #region Commands

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(RefreshList);
            }
        }
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
