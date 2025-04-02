using macaroni_dev.Services;
using macaroni_dev.Views;
using Supabase.Gotrue;
namespace macaroni_dev;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    protected override async void OnStart()
    {
        base.OnStart();
        var sessionToken = await SecureStorage.GetAsync("session_token");
        var refreshToken = await SecureStorage.GetAsync("refresh_token");
        if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(refreshToken))
        {
            var authService = await AuthService.GetInstanceAsync();
            var user = await authService.RestoreSessionAsync(sessionToken, refreshToken);
            if (user != null)
            {
                Console.WriteLine(user.Email);
                MainPage = new AppShell();
                return;
            }
        }
        MainPage = new NavigationPage(new LoginPage());
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new LoadingPage()); // Temporary page while checking login
    }
}
