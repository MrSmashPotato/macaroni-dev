using macaroni_dev.Services;
using macaroni_dev.Views;
using Mopups.Services;
using Supabase.Gotrue;

namespace macaroni_dev.ViewModels;

public class HomePageViewModel
{

    
    public HomePageViewModel()
    {
        var profile = ServiceHelper.GetService<ProfileService>();
        var user = profile.CurrentUser;
        if (user != null && user.IsComplete == false)
        {
            MopupService.Instance.PushAsync(new CompleteRegistrationMopup());
        } else if (user != null && user.IsComplete == true)
        {
            
        }
        else
        {
            Console.WriteLine("User is not found");
        }
    }
}