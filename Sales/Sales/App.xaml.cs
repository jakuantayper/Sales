namespace Sales
{
    using Views;
    using ViewModels;
    using Helpers;
    using Xamarin.Forms;

    public partial class App : Application
    {
        public static NavigationPage Navigator { get; internal set; }

        public App()
        {
            InitializeComponent();

            if (Settings.IsRemembered && !string.IsNullOrEmpty(Settings.AccessToken))
            {
                MainViewModel.GetInstance().Products = new ProductsViewModel();
                MainPage = new MasterPage();
            }
            else
            {
                MainViewModel.GetInstance().Login = new LoginViewModel();
                MainPage = new NavigationPage(new LoginPage());
            }
           
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {  
        }

        protected override void OnResume()
        {
        }
    }
}
