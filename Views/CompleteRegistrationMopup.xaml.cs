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

	public void Exit(Object o, EventArgs e)
	{
		_homePageViewModel.Blur = false;
		MopupService.Instance.PopAsync();
	}
}
