using macaroni_dev.Services;
using Microsoft.Maui.Controls;
using Supabase.Gotrue;
using Supabase.Postgrest.Exceptions;

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
                Application.Current.MainPage = new AppShell();
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
                else if (provider == "Github")
                {
                    TProvider = Constants.Provider.Github;
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
                    Application.Current.MainPage = new AppShell();
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