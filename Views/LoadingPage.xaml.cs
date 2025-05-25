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
        private readonly AuthService _authService;

        public LoadingPage()
        {
            InitializeComponent();
            Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
            Shell.SetNavBarIsVisible(this, false);
            _authService = ServiceHelper.GetService<AuthService>();
        }

        protected override  void OnAppearing()
        {
            base.OnAppearing();
            CheckLoginStatus();

        }
        private async void CheckLoginStatus()
        {
            try
            {
                var sessionToken = await SecureStorage.GetAsync("session_token");
                var refreshToken = await SecureStorage.GetAsync("refresh_token");

                if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    int attempts = 0;
                    const int maxAttempts = 5;

                    while (attempts < maxAttempts)
                    {
                        var user = await _authService.RestoreSessionAsync(sessionToken, refreshToken);
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
                            return;
                        }

                        attempts++;
                        await Task.Delay(500); // Optional delay
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring session: {ex.Message}");
            }

            Application.Current.MainPage = new LoginPage();
        }
    }
}
