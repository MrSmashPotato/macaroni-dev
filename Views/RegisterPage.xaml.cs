using macaroni_dev.Models;
using macaroni_dev.Services;

namespace macaroni_dev.Views
{
    public partial class RegisterPage : ContentPage
    {
        private AuthService _authService;
        public RegisterPage()
        {
            InitializeComponent();
            LoadAuthService();
        }
        private async void LoadAuthService()
        {
            _authService = ServiceHelper.GetService<AuthService>();
        }

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            var username = UserEntry.Text;
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please fill all fields", "OK");
                return;
            }

            var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
            var result = await client
                .From<User>()
                .Where(x => x.Username == username || x.EmailAddress == email)
                .Get();

            bool usernameTaken = result.Models.Any(val => val.Username == username);
            bool emailTaken = result.Models.Any(val => val.EmailAddress == email);

            if (usernameTaken && emailTaken)
            {
                await DisplayAlert("Both Username and Recovery Email are already taken", "Use only unused Username and Recovery Email", "Ok");
                return;
            }
            else if (usernameTaken)
            {
                await DisplayAlert("Username already taken", "Use a different Username", "Ok");
                return;
            }
            else if (emailTaken)
            {
                await DisplayAlert("Recovery Email already in use", "Use a different Recovery Email", "Ok");
                return;
            }

            // ✅ If checks pass, proceed with signup
            var user = await _authService.SignUpAsync(username, email, password);

            if (user != null)
            {
                await DisplayAlert("Sign-up successful", "Verification email sent. Please check your email.", "Ok");
                await Navigation.PushAsync(new OtpVerificationPage(email));
            }
            else
            {
                StatusLabel.Text = "Sign-up failed. Please try again later.";
            }
        }

        private async void OnGoToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}