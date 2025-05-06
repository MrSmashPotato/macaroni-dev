using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Supabase.Postgrest.Exceptions;
using Supabase.Postgrest.Responses;

namespace macaroni_dev.ViewModels;

public partial class PostApplyViewModel : ObservableObject
{
    [ObservableProperty] private JobPost _job;
    [ObservableProperty] private Skill _skill;
    [ObservableProperty] private bool _saveable = true;
    private Supabase.Client client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
    private HomePageViewModel _homePage;
    public PostApplyViewModel(JobPost post, Skill skill, HomePageViewModel homePageViewModel)
    {
        Job = post;
        Skill = skill;
        _homePage = homePageViewModel;
        if (_homePage.SavedJobs.Any(j => j.Id == post.Id))
        {
            Saveable = false;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            var client = ServiceHelper.GetService<SupabaseClientProvider>();
            var user_id = ServiceHelper.GetService<ProfileService>().CurrentUser.ID;
            var post_id = Job.Id;

            var savedPost = new JobSaved
            {
                UserId = user_id,
                PostId = post_id,
                SavedAt = DateTime.UtcNow
            };

            ModeledResponse<JobSaved> result;
            try
            { 
                result = await client.GetSupabaseClient().From<JobSaved>().Insert(savedPost);
                if (result.Model != null)
                {
                    _homePage.SavedJobs.Add(Job);
                    Saveable = false;
                }
            }
            catch (PostgrestException e)
            {
                Console.WriteLine("Error database" + e.Message);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving post: {ex.Message}");
        }
    }
    [RelayCommand]
    private async Task Apply()
    {
        var user_id = ServiceHelper.GetService<ProfileService>().CurrentUser.ID;
        var job_id = Job.Id;

        JobApplication application = new JobApplication()
        {
            JobId = job_id,
            UserId = user_id,
            CreatedAt = DateTime.UtcNow,
            IsRejected = false,
            IsArchived = false,
            Status = "Pending",
        };
        
        var result = await client.From<JobApplication>().Insert(application);
        await ShowAlertAsync("Success", "Job application succesfully submitted");
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    private async Task ShowAlertAsync(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}