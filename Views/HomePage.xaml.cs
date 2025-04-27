using macaroni_dev.Services;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            BindingContext = new HomePageViewModel();
            InitializeComponent();
        }
    }
}