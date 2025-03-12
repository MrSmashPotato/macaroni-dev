using macaroni_dev.Services;

namespace macaroni_dev
{
    public partial class RegisterPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegisterPage()
        {
            InitializeComponent();
            _authService = AuthService.GetInstanceAsync().Result;
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            var user = await _authService.SignUpAsync(EmailEntry.Text, PasswordEntry.Text);

            if (user != null)
            {
                await Navigation.PushAsync(new OtpVerificationPage(EmailEntry.Text));
            }
            else
            {
                StatusLabel.Text = "Sign-up failed. Please try again.";
            }
        }

        private async void OnGoToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}