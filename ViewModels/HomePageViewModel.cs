using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.Views;
using macaroni_dev.Views.LoadingPopup;
using Mopups.Services;
using Supabase.Gotrue;

namespace macaroni_dev.ViewModels;

public partial class HomePageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<JobPost> _jobFeed;
    [ObservableProperty]
    private ObservableCollection<JobPost> _savedJobs = new ObservableCollection<JobPost>();
    [ObservableProperty] 
    private bool _owned;
    

    [ObservableProperty]
    private bool _blur;
    public HomePageViewModel()
    {
        Blur = false;
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

    public async Task FetchAsync()
    {
        var profile = ServiceHelper.GetService<ProfileService>();
        var currentUserId = profile.CurrentUser.ID;

        try
        {
            var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
            var jobResponse = await client
                .From<JobPost>()
                .Get();

            if (jobResponse.Models.Count > 0)
            {
                var applicationResponse = await client
                    .From<JobApplication>()
                    .Where(x => x.UserId == currentUserId)
                    .Get();

                var appliedJobIds = applicationResponse.Models
                    .Select(app => app.JobId)
                    .ToHashSet(); 

                foreach (var job in jobResponse.Models)
                {
                    job.IsApplied = appliedJobIds.Contains(job.Id);
                }

                JobFeed = new ObservableCollection<JobPost>(jobResponse.Models);
            }
            
            var saved = await client
                .From<JobSaved>()
                .Where(x => x.UserId == profile.CurrentUser.ID)
                .Get();
            if (saved.Models.Count > 0 && JobFeed.Count > 0)
            {
                var savedPosts = JobFeed
                    .Where(post => saved.Models.Any(savedItem => savedItem.PostId == post.Id))
                    .ToList();
                SavedJobs = new ObservableCollection<JobPost>(savedPosts);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching job feed: {ex.Message}");
        }       
    }
    [RelayCommand]
    private async Task Apply(JobPost jobPost)
    {
        await MopupService.Instance.PushAsync(new ProcessingPopup());

        var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        var PostSkill = await client.From<JobSkill>().Where(x => x.JobID == jobPost.Id).Get();
        var Skill = await client.From<Skill>().Where(x => x.ID == PostSkill.Model.SkillID).Get();
        await Application.Current.MainPage.Navigation.PushAsync(new Views.PostApplyPage(jobPost, Skill.Model, this));
        await MopupService.Instance.PopAsync();

    }
    [RelayCommand]
    private async Task Edit(JobPost jobPost)
    {
        await MopupService.Instance.PushAsync(new ProcessingPopup());
        var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        var PostSkill = await client.From<JobSkill>().Where(x => x.JobID == jobPost.Id).Get();
        var Skill = await client.From<Skill>().Where(x => x.ID == PostSkill.Model.SkillID).Get();
        await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.JobPost(jobPost, Skill.Model));
        await MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    private async Task RemoveSaved(JobPost jobPost)
    {
        await MopupService.Instance.PushAsync(new ProcessingPopup());
        var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        var user_id = ServiceHelper.GetService<ProfileService>().CurrentUser.ID;
        await client.From<JobSaved>().Where(x => x.UserId == user_id && x.PostId == jobPost.Id).Delete();
        var itemToRemove = SavedJobs.FirstOrDefault(x => x.Id == jobPost.Id);
        if (itemToRemove != null)
        {
            SavedJobs.Remove(itemToRemove);
            Console.WriteLine("Removed job post");
        }
        await ShowAlertAsync("Success", "Saved Job Successfully removed");
        await MopupService.Instance.PopAsync();
    }
    private async Task ShowAlertAsync(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}