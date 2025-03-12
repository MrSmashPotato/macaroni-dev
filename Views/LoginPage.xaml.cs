using macaroni_dev.Services;
using Microsoft.Maui.Controls;
using Supabase.Postgrest.Exceptions;

namespace macaroni_dev
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = AuthService.GetInstanceAsync().Result; 
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            try
            {
                var user = await _authService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);
                
                if (user != null)
                {
                    await Navigation.PushAsync(new HomePage());
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

        private async void OnGoToRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}