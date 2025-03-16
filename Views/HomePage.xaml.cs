using macaroni_dev.Services;

namespace macaroni_dev.Views
{
    public partial class HomePage : ContentPage
    {
        private AuthService _authService;

        public HomePage()
        {
            InitializeComponent();
            LoadAuthService();
            try
            {
                Console.WriteLine(_authService?.GetCurrentUser()?.Email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private async void LoadAuthService()
        {
            _authService = await AuthService.GetInstanceAsync();
        }
        
    }
}