using macaroni_dev.Services;
using macaroni_dev.Views;

namespace macaroni_dev;

public partial class AppShell : Shell
{
	private AuthService _authService = ServiceHelper.GetService<AuthService>();
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("homePage", typeof(HomePage));
		
	}
	
	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		bool confirm = await Application.Current.MainPage.DisplayAlert(
			"Logout", "Are you sure you want to log out?", "Yes", "No");
		if (confirm)
		{
			try
			{
				// Sign out 
				await _authService.SignOutAsync();
				Application.Current.MainPage = new NavigationPage(new LoginPage());
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Logout failed: {ex.Message}", "OK");
			}
		}
	}
}
