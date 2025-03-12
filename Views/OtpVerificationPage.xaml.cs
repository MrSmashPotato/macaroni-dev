using macaroni_dev.Services;

namespace macaroni_dev
{
    public partial class OtpVerificationPage : ContentPage
    {
        private readonly string _email;
        private  AuthService _authService;

        public OtpVerificationPage(string email)
        {
            InitializeComponent();
            _email = email;
            Console.WriteLine(_email);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _authService = await AuthService.GetInstanceAsync();
        }
        private async void OnVerifyClicked(object sender, EventArgs e)
        {
            string otpCode = OtpEntry.Text;

            if (string.IsNullOrWhiteSpace(otpCode))
            {
                StatusLabel.Text = "Please enter the OTP.";
                return;
            }

            bool isVerified = await _authService.VerifyEmailOtpAsync(_email, otpCode);

            if (isVerified)
            {
                StatusLabel.Text = "✅ Verification successful! You can now sign in.";
                await Navigation.PopToRootAsync(); // Go back to MainPage
            }
            else
            {
                StatusLabel.Text = "❌ Invalid OTP. Please try again.";
            }
        }
    }
}