using macaroni_dev.Services;

namespace macaroni_dev
{
    public partial class HomePage : ContentPage
    {
        private readonly AuthService _authService;

        public HomePage()
        {
            InitializeComponent();
            _authService = AuthService.GetInstanceAsync().Result;
            try
            {
                Console.WriteLine(_authService?.GetCurrentUser()?.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            bool success = await _authService.SignOutAsync();
            if (success)
            {
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                StatusLabel.Text = "Sign-out failed.";
            }
        }
    }
}