using macaroni_dev.ViewModels;
using macaroni_dev.Views.Profile;

namespace macaroni_dev.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel(); // Set the BindingContext to the ViewModel
    }
}