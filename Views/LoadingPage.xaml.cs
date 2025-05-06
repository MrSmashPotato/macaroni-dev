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
        private AuthService _authService;

        public LoadingPage()
        {
            InitializeComponent();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            Shell.SetNavBarIsVisible(this, false);
            CheckLoginStatus();
        }

        private async void CheckLoginStatus()
        {
            var sessionToken = await SecureStorage.GetAsync("session_token");
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var authService = ServiceHelper.GetService<AuthService>();
                var user = await authService.RestoreSessionAsync(sessionToken, refreshToken);
                if (user != null)
                {
                    if (user.ConfirmedAt == null)
                    {
                        await Navigation.PushAsync(new OtpVerificationPage(user.Email));
                    }
                    else
                    {
                        var profile = ServiceHelper.GetService<ProfileService>();
                        await profile.InitializeProfileAsync(user.Id);
                        await Shell.Current.GoToAsync("//homePage");

                    }
                }
                else
                {
                    await Shell.Current.GoToAsync("//loginPage");  
                }
            }

        }
    }
}

