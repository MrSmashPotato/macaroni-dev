using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace macaroni_dev.Views;
using macaroni_dev.Services;
public partial class LoadingPage : ContentPage
{
    public LoadingPage()
    {
        InitializeComponent();
        CheckLoginStatus();
    }

    private async void CheckLoginStatus()
    {
        var authService = await AuthService.GetInstanceAsync(); // Get your service instance

        bool isLoggedIn = authService.IsUserLoggedIn(); // Implement this in AuthService

        // Navigate to the correct page
        if (isLoggedIn)
            Application.Current.MainPage = new AppShell();
        else
            Application.Current.MainPage = new NavigationPage(new LoginPage());    }
}