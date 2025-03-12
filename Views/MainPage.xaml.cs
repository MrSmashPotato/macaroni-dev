using macaroni_dev.Services;

namespace macaroni_dev
{
    public partial class MainPage : ContentPage
    {
        private AuthService _authService;

        public MainPage()
        {
            InitializeComponent();
            InitializeAuthService();
        }

        private async void InitializeAuthService()
        {
            _authService = await AuthService.GetInstanceAsync();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            var authService = await AuthService.GetInstanceAsync();
            var user = await authService.SignUpAsync(EmailEntry.Text, PasswordEntry.Text);

            if (user != null)
            {
                await Navigation.PushAsync(new OtpVerificationPage(EmailEntry.Text));
            }
            else
            {
                StatusLabel.Text = "Sign-up failed. Please try again.";
            }
        }
        private async void OnSignInClicked(object sender, EventArgs e)
        {
            var user = await _authService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);
            StatusLabel.Text = user != null ? $"Welcome, {user.Email}" : "Sign-in failed.";
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool success = await _authService.SignOutAsync();
            StatusLabel.Text = success ? "Signed out." : "Sign-out failed.";
        }
    }
}