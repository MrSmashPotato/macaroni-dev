using macaroni_dev.Services;
using macaroni_dev.Views;

namespace macaroni_dev;

public partial class AppShell : Shell
{
	private AuthService _authService;
	private Color defaultBackground = Colors.White; // Default Page Background
	private Color dimmedBackground = Color.FromArgb("#B3000000"); //Semi-Transparent Page Effect
	private Color defaultShellColor = Colors.MediumAquamarine;

	public AppShell()
	{
		InitializeComponent();
		LoadAuthService();
		this.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == nameof(FlyoutIsPresented))
			{
				UpdateFlyoutShadow();
			}
		};
	}
	private void UpdateFlyoutShadow()
	{
		if (FlyoutIsPresented)
		{
			Shell.SetBackgroundColor(CurrentPage, dimmedBackground);
			CurrentPage.BackgroundColor = dimmedBackground; // Dim background when flyout opens
		}
		else
		{
			Shell.SetBackgroundColor(CurrentPage, defaultShellColor);
			CurrentPage.BackgroundColor = defaultBackground; // Restore original background
		}
	}
	private async void LoadAuthService()
	{
		_authService = await AuthService.GetInstanceAsync();
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
