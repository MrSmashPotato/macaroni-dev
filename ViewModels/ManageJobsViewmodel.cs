using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.Views.LoadingPopup;
using Mopups.Services;

namespace macaroni_dev.ViewModels;

public partial class ManageJobsViewmodel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<JobPost> _myPosts;

    [ObservableProperty]
    private bool _noPost = false;

    [ObservableProperty] private bool _isEditEnabled = true;
    public ManageJobsViewmodel()
    {
        MyPosts = new ObservableCollection<JobPost>();
    }

    [RelayCommand]
    private async Task EditButton(JobPost jobPost)
    {
        await MopupService.Instance.PushAsync(new ProcessingPopup());

        IsEditEnabled = true;
        var client = ServiceHelper.GetService<SupabaseClientProvider>();
        var PostSkill = await client.GetSupabaseClient().From<JobSkill>().Where(x => x.JobID == jobPost.Id).Get();
        var Skill = await client.GetSupabaseClient().From<Skill>().Where(x => x.ID == PostSkill.Model.SkillID).Get();
        await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.JobPost(jobPost, Skill.Model));
        await MopupService.Instance.PopAsync();
    }

    public async Task LoadMyPosts()
    {
        try
        {
            var supabase = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
            var u_id = Guid.Parse(ServiceHelper.GetService<AuthService>().GetCurrentUser().Id);
            

            var response = await supabase
                .From<JobPost>()
                .Where(post => post.UserID == u_id) // Filter by user_id
                .Get(); // Fetch the records


            var posts = response.Models;
            if (posts.Count > 0)
            {
                MyPosts.Clear(); // Clear existing posts (if needed)
                foreach (var post in posts)
                {
                    MyPosts.Add(post); // Add fetched posts to the ObservableCollection
                }
                NoPost = false;
            }
            else
            {
                NoPost = true;
                Console.WriteLine("No posts found");
            }
            Console.WriteLine("Result Values");
            Console.WriteLine(MyPosts[0].JobName);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        
        
    }
}