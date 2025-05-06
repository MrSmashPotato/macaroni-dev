using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Mopups.Services;

namespace macaroni_dev.ViewModels;

public partial class EditJobApplicationViewModel : ObservableObject
{
    private User _user;
    [ObservableProperty] private string _status = String.Empty;
    [ObservableProperty] private string _statusError = String.Empty;
    JobApplicantsViewModel viewModel;
    private Supabase.Client _client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
    JobPost jobPost;

    public EditJobApplicationViewModel(User user, JobApplicantsViewModel vm, JobPost jobPost)
    {
        this.jobPost = jobPost;
        viewModel = vm;
        _user = user;
        Status = _user.Application.Status;
    }
    
    [RelayCommand]
    private void ClearStatus()
    {
        Status = string.Empty;
        StatusError = string.Empty;
    }
    
    [RelayCommand]
    private async Task Reject()
    {
      await _client.From<JobApplication>().Where(x => x.Id == _user.Application.Id).Delete();
      await ShowAlertAsync("Success", "Job application rejected");
      await Application.Current.MainPage.Navigation.PopAsync();
      await viewModel.CheckCommand.ExecuteAsync(jobPost);
      await MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    private async Task Update()
    {
        var result = await _client.From<JobApplication>().Where(x => x.Id == _user.Application.Id).Single();
       if (result != null)
       {
           result.Status = Status;
           _user.Application.Status = result.Status;
           await result.Update<JobApplication>();
           await ShowAlertAsync("Success", "Job application updated");
           await MopupService.Instance.PopAsync();
       }
       else
       {
           await ShowAlertAsync("Error", "Job application not found");
       }
    }
    private async Task ShowAlertAsync(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}