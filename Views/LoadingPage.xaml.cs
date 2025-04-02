using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace macaroni_dev.Views
{
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
            CheckLoginStatus();
        }

        private async void CheckLoginStatus()
        {
            var sessionToken = await SecureStorage.GetAsync("session_token");
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var authService = await AuthService.GetInstanceAsync();
                var user = await authService.RestoreSessionAsync(sessionToken, refreshToken);
                if (user != null)
                {
                    Console.WriteLine(user.Email);
                    Application.Current.MainPage = new AppShell();
                    return;
                }
            }
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
