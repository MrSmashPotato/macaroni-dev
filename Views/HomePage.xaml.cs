using macaroni_dev.Services;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views
{
    public partial class HomePage : ContentPage
    {
        private HomePageViewModel vm;
        public HomePage()
        {
            vm = new HomePageViewModel();
            BindingContext = vm;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.FetchAsync();
        }
    }
}