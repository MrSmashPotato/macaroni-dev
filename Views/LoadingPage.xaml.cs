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
                    var isEmailRegistered = await IsEmailRegisteredAsync(user.Email);

                    if (isEmailRegistered == true)
                    {
                        // Navigate to AppShell if the email is already registered
                        Application.Current.MainPage = new AppShell();
                    }
                    else
                    {
                        // Navigate to CompleteRegistrationPage if the email is not registered
                        await Navigation.PushAsync(new CompleteRegistrationPage());
                    }
                    return;
                }
            }
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
        private async Task<bool> IsEmailRegisteredAsync(string email)
        {
            try
            {
                // Query the public.User table for the email
                var users = await _authService.GetSupabaseClient()
                    .From<Models.User>() // Use the correct User model
                    .Filter("EmailAddress", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Get();

                return users.Models.Count > 0; // Return true if the user exists, false otherwise
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email registration: {ex.Message}");
                return false;
            }
        }
    }
}
