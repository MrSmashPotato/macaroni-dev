using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Supabase.Postgrest;

namespace macaroni_dev.ViewModels;

public partial class AppliedJobsViewmodel : ObservableObject
{
    Supabase.Client client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
    public ObservableCollection<JobPost> AppliedJobs { get; set; } = new ObservableCollection<JobPost>();
    
    
    
    [ObservableProperty]
    private bool _haveApplied = false;
    public AppliedJobsViewmodel()
    {
        
    }
    
    public async Task Fetch()
    {

        var userId = ServiceHelper.GetService<ProfileService>().CurrentUser.ID;

        var applicationResult = await client
            .From<JobApplication>()
            .Where(x => x.UserId == userId)
            .Get();

        var applications = applicationResult.Models;

        if (!applications.Any())
        {
            AppliedJobs.Clear();
            HaveApplied = false;
            return;
        }

        var jobIds = applications.Select(app => app.JobId).ToList();

        var jobPostResult = await client
            .From<JobPost>()
            .Filter("id", Constants.Operator.In, jobIds)
            .Get();

        if (!jobPostResult.Models.Any())
        {
            HaveApplied = false;
            return;
        }

        AppliedJobs.Clear();
        foreach (var job in jobPostResult.Models)
        {
            var matchingApp = applications.FirstOrDefault(app => app.JobId == job.Id);
            job.Application = matchingApp;

            AppliedJobs.Add(job);
        }

        HaveApplied = true;
    }

   
    [RelayCommand]
    private async Task Withdraw(JobPost post)
    {
        var appliedto = post.Application;
        if (appliedto != null)
        {
            await client.From<JobApplication>().Where(x => x.JobId == appliedto.JobId).Delete();
            await ShowAlertAsync("Success", "Job application updated");
            await Fetch();
        }
        else
        {
            Console.WriteLine("Applied job id is null");
        }
    }
    private async Task ShowAlertAsync(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}