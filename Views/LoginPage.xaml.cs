using macaroni_dev.Services;
using Microsoft.Maui.Controls;
using Supabase.Gotrue;
using Supabase.Postgrest.Exceptions;
using macaroni_dev.Models;

namespace macaroni_dev.Views
{
    public partial class LoginPage : ContentPage
    {
        private AuthService _authService;
        
        public LoginPage()
        {
            InitializeComponent();
            LoadAuthService();
        }
        private async void LoadAuthService()
        {
            _authService = await AuthService.GetInstanceAsync();
        }
        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                var user = await _authService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);

                if (user == null) return;
                Console.WriteLine(user.Email);
                var isEmailRegistered = await IsEmailRegisteredAsync(user.Email);

                if (isEmailRegistered)
                {
                    // Navigate to AppShell if the email is already registered
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    // Navigate to CompleteRegistrationPage if the email is not registered
                    await Navigation.PushAsync(new CompleteRegistrationPage());
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("email_not_confirmed"))  // Check for confirmation error
                {
                    await DisplayAlert("Email Not Confirmed", 
                        "Please confirm your email before signing in.", "OK");

                    await Navigation.PushAsync(new OtpVerificationPage(EmailEntry.Text));
                }
                else
                {
                    StatusLabel.Text = "Sign-in failed: " + ex.Message;
                }
            }
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
        private void PartySignInClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is string provider)
            {
                Constants.Provider TProvider;
                if (provider == "Google")
                {
                    TProvider = Constants.Provider.Google;
                }
                else if (provider == "Facebook")
                {
                    TProvider = Constants.Provider.Facebook;
                }
                else
                {
                    return;
                }
                SignInWithProvider(TProvider);
            }
        }
        private async void SignInWithProvider(Constants.Provider provider)
        {
            try
            {
                var user = await _authService.SignInWithThirdPartyAsync(provider);
                if (user != null)
                {
                    Console.WriteLine("Third Party SignIn Success");
                    var isEmailRegistered = await IsEmailRegisteredAsync(user.Email);

                    if (isEmailRegistered)
                    {
                        // Navigate to AppShell if the email is already registered
                        Application.Current.MainPage = new AppShell();
                    }
                    else
                    {
                        // Navigate to CompleteRegistrationPage if the email is not registered
                        await Navigation.PushAsync(new CompleteRegistrationPage());
                    }
                }
                else
                {
                    StatusLabel.Text = "Third Party Sign-In failed. Try again.";
                }
            }
            catch (Exception m)
            {
               Console.WriteLine(m.Message);
            }
        }
      
        private async void OnGoToRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}