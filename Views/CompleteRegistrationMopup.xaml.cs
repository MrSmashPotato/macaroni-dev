using macaroni_dev.ViewModels;
using Mopups.Services;

namespace macaroni_dev.Views;

public partial class CompleteRegistrationMopup
{
	HomePageViewModel _homePageViewModel;
	public CompleteRegistrationMopup(HomePageViewModel viewModel)
	{
		_homePageViewModel = viewModel;
		_homePageViewModel.Blur = true;
		InitializeComponent();
	}

	protected override void OnDisappearing()
	{
		_homePageViewModel.Blur = false;
	}

	public void Exit(Object o, EventArgs e)
	{
		_homePageViewModel.Blur = false;
		MopupService.Instance.PopAsync();
	}
	private async void OnInputFocused(object sender, FocusEventArgs e)
	{
		await Task.Delay(100);
    
		
			await MainScrollView.ScrollToAsync(sender as Entry, ScrollToPosition.Start, true);
		
		
	}
}
