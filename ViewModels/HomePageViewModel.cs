using CommunityToolkit.Mvvm.ComponentModel;
using macaroni_dev.Services;
using macaroni_dev.Views;
using Mopups.Services;
using Supabase.Gotrue;

namespace macaroni_dev.ViewModels;

public partial class HomePageViewModel : ObservableObject
{

    [ObservableProperty]
    private bool _blur = false;
    public HomePageViewModel()
    {
        var profile = ServiceHelper.GetService<ProfileService>();
        var user = profile.CurrentUser;
        if (user != null && user.IsComplete == false)
        {
            MopupService.Instance.PushAsync(new CompleteRegistrationMopup(this));
        } else if (user != null && user.IsComplete == true)
        {
            
        }
        else
        {
            Console.WriteLine("User is not found");
        }
    }
}