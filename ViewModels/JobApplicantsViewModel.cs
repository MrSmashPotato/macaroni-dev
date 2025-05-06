using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.Views.LoadingPopup;
using macaroni_dev.Views.Popup;
using macaroni_dev.Views.Profile;
using Mopups.Services;
using Supabase.Postgrest;
using JobPost = macaroni_dev.Models.JobPost;

namespace macaroni_dev.ViewModels;

public partial class JobApplicantsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<JobPost> _myPosts = new ObservableCollection<JobPost>();
    [ObservableProperty]
    private ObservableCollection<User> _myApplicants = new ObservableCollection<User>();
    [ObservableProperty]
    private bool _noPost = false;
    [ObservableProperty]
    private bool _noApplicant = false;
    private Supabase.Client _client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
    JobPost _jobPost;

    public JobApplicantsViewModel()
    {
    }
    
    [RelayCommand]
    private async Task Check(JobPost post)
    {
        _jobPost = post;
        var result = await _client.From<JobApplication>().Where(j => j.JobId == post.Id).Get();
        var obs = new ObservableCollection<JobApplication>(result.Models);
        await Application.Current.MainPage.Navigation.PushAsync(new Applicants(obs, this));
    }

    [RelayCommand]
    private async Task Update(User user)
    {
        await MopupService.Instance.PushAsync(new EditJobApplicationPopup(user, this, _jobPost));
    }
    
    public async Task LoadMyPosts()
    {
        try
        {
            var u_id = Guid.Parse(ServiceHelper.GetService<AuthService>().GetCurrentUser().Id);
            

            var response = await _client
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
    

    public async Task LoadMyApplicants(ObservableCollection<JobApplication> applicants)
    {
        MyApplicants.Clear();

        var userIds = applicants.Select(a => a.UserId).Distinct().ToList();
        var response = await _client
            .From<User>()
            .Filter("id", Constants.Operator.In, userIds)
            .Get();
        if (response.Models.Count > 0)
        {
            var users = response.Models;
            foreach (var user in users)
            {
                user.Application = applicants.FirstOrDefault(a => a.UserId == user.ID);
            }

            MyApplicants = new ObservableCollection<User>(users);
        }
        else
        {
            Console.WriteLine("Users not found");
        }
    }
}