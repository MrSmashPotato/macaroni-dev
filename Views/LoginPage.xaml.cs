using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Services;
using Microsoft.Maui.Controls;
using Supabase.Gotrue;
using Supabase.Postgrest.Exceptions;
using macaroni_dev.Models;
using Microsoft.IdentityModel.Tokens;

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
            _authService = ServiceHelper.GetService<AuthService>();
        }
      private async void OnSignInClicked(object sender, EventArgs e)
{
    var email = EmailEntry.Text;
    var password = PasswordEntry.Text;

    if (email.IsNullOrEmpty() || password.IsNullOrEmpty())
    {
        StatusLabel.Text = "Please fill all fields";
        return;
    }

    int maxAttempts = 5;
    int attempts = 0;

    while (attempts < maxAttempts)
    {
        try
        {
            StatusLabel.Text = $"Signing in... (Attempt {attempts + 1}/{maxAttempts})";

            var user = await _authService.SignInAsync(email, password);

            if (user == null)
            {
                StatusLabel.Text = "Sign-in failed. Please try again.";
                return;
            }

            Console.WriteLine(user.Email);
            var profile = ServiceHelper.GetService<ProfileService>();
            await profile.InitializeProfileAsync(user.Id);

            var curruser = _authService.GetCurrentUser();
            if (curruser.ConfirmedAt == null)
            {
                await Navigation.PushAsync(new OtpVerificationPage(curruser.Email));
            }
            else
            {
                Application.Current.MainPage = new AppShell();
                await Shell.Current.GoToAsync("//homePage");
            }

            return; // success
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("email_not_confirmed"))
            {
                await DisplayAlert("Email Not Confirmed", 
                    "Please confirm your email before signing in.", "OK");

                await Navigation.PushAsync(new OtpVerificationPage(email));
                return;
            }

            // Retry on transient errors only
            if (IsTransientError(ex))
            {
                attempts++;
                await Task.Delay(800); // Optional delay between retries
                continue;
            }
            else
            {
                StatusLabel.Text = "Sign-in failed: " + ex.Message.Replace("error: ", "");
                return;
            }
        }
    }

    StatusLabel.Text = "Failed to sign in after multiple attempts. Please try again later.";
}

private async void Forgot(Object sender, EventArgs e)
{
    await Navigation.PushAsync(new ForgotPage());
    
}
private bool IsTransientError(Exception ex)
{
    return ex is TimeoutException ||
           ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
           ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase) ||
           ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase);
}
        private async Task<bool> IsEmailRegisteredAsync(string email)
        {
            try
            {
                var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
                // Query the public.User table for the email
                var users = client
                    .From<Models.User>() // Use the correct User model
                    .Filter("EmailAddress", Supabase.Postgrest.Constants.Operator.Equals, email)
                    .Get();

                return users.Result.Models.Count > 0; // Return true if the user exists, false otherwise
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
                    var profile = ServiceHelper.GetService<ProfileService>();
                    await profile.InitializeProfileAsync(user.Id);
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync("//homePage");
    
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