using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.Views;
using macaroni_dev.Views.LoadingPopup;
using Mopups.Services;
using Supabase.Gotrue;
using Supabase.Postgrest.Responses;
using Constants = Supabase.Postgrest.Constants;

namespace macaroni_dev.ViewModels;

public partial class HomePageViewModel : ObservableObject
{
    private string filterMode = "No Filter";
    [ObservableProperty]
    private List<Skill> _skillsList = new List<Skill>();
    [ObservableProperty]
    private int _filterSkillId = 0;
    [ObservableProperty]
    private decimal _filterStartSalary = 0;
    [ObservableProperty]
    private decimal _filterEndSalary = 0;

    [ObservableProperty] private string _filterLocation = "";
    private ObservableCollection<JobPost> _container; 
    [ObservableProperty]
    private ObservableCollection<JobPost> _jobFeed;
    [ObservableProperty]
    private ObservableCollection<JobPost> _savedJobs = new ObservableCollection<JobPost>();
    [ObservableProperty] 
    private bool _owned;
    [ObservableProperty]
    private string _searchText;
    [ObservableProperty]
    private List<JobPost> _searchResults = new List<JobPost>();
    
    [ObservableProperty] private JobPost _selectedSearch;

    partial void OnSelectedSearchChanged(JobPost? oldval, JobPost newval)
    {
        ApplyCommand.ExecuteAsync(newval);
    }
    partial void OnSearchTextChanged(string? oldval, string newval)
    {
        Task.Run(async () =>
        {
            var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
            
            var res = await client
                .From<JobPost>()
                .Filter(x => x.JobName, Constants.Operator.ILike, $"%{SearchText}%")
                .Get();
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SearchResults = res.Models;
            });
        });

    }
    
    
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

    public async Task ChangeFilter(string filter)
    {
        JobFeed.Clear();
        if (filter == "No Filter")
        {
            _container = JobFeed;
            filterMode = filter;
            await FetchAsync();
            return;
        } 
        filterMode = filter;
        await FetchAsync();
        
    }

    public async Task FetchSkills()
    {
        var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        var res = await client.From<Skill>().Get();
        SkillsList = res.Models;
    }
    public async Task FetchAsync()
    {
        var profile = ServiceHelper.GetService<ProfileService>();
        var currentUserId = profile.CurrentUser.ID;

        try
        {
            var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
            ModeledResponse<JobPost> jobResponse = null;
            if (filterMode == "No Filter")
            {
                 jobResponse = await client
                    .From<JobPost>()
                    .Get();
            }
            else if (filterMode == "Filter by Skills")
            {
                try
                {
                    var js = await client.From<JobSkill>()
                        .Where(x => x.SkillID == FilterSkillId)
                        .Get();

                    // Ensure js.Models is not empty before building the IN filter
                    var jobIds = js.Models.Select(n => n.JobID).ToList();
                    if (jobIds.Count == 0)
                    {
                        Console.WriteLine("No job IDs found for the selected skill.");
                    }
                    else
                    {
                        jobResponse = await client
                            .From<JobPost>()
                            .Filter(x => x.Id, Constants.Operator.In, jobIds)
                            .Get();

                        Console.WriteLine("Gabii Count: " + jobResponse.Models.Count);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while filtering by skills: " + ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
            else if (filterMode == "Filter by Location")
            {
                jobResponse = await client.From<JobPost>().Filter(x => x.Location, Constants.Operator.ILike, FilterLocation).Get();
            } else if (filterMode == "Filter by Salary")
            {
                Console.WriteLine("Salary Filteringers");
                jobResponse = await client.From<JobPost>()
                    .Filter(x => x.Salary, Constants.Operator.GreaterThanOrEqual, (float)FilterStartSalary)
                    .Filter(x => x.Salary, Constants.Operator.LessThanOrEqual, (float)FilterEndSalary)
                    .Get();
                Console.WriteLine("Salary Filtered: " + jobResponse.Models.Count);
            }
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