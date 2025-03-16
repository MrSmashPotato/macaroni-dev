using macaroni_dev.Views;
namespace macaroni_dev;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
	}
	
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new LoadingPage()); // Temporary page while checking login
	}
}