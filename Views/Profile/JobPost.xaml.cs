using System.ComponentModel;
using macaroni_dev.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using macaroni_dev.Messengers;
using Syncfusion.Maui.Core;

namespace macaroni_dev.Views.Profile;

public partial class JobPost : ContentPage
{
	private ViewModels.JobPostViewModel vm;
	private Models.JobPost jobPost;
	public JobPost()
	{
		InitializeComponent();
		jobPost = null;
		CreateButton.IsVisible = true;
		vm = new JobPostViewModel();
		CreateLabel.IsVisible = true;
		EditLabel.IsVisible = false;
		BindingContext = vm;
	}
	
	
	public JobPost(Models.JobPost jobPost, Models.Skill skill)
	{
		this.jobPost = jobPost;
		try
		{
			InitializeComponent();
        
			if (jobPost == null)
			{
				throw new ArgumentNullException(nameof(jobPost), "JobPost parameter is null");
			}

			EditButton.IsVisible = true;
			DeleteButton.IsVisible = true;
			EditLabel.IsVisible = true;
			CreateLabel.IsVisible = false;
			vm = new JobPostViewModel(jobPost, skill);
			Console.WriteLine("The Job Post Id is:" + jobPost.Id);
			BindingContext = vm;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in JobPost constructor: {ex.Message}");
		}
	}
	private async void OnInputFocused(object sender, FocusEventArgs e)
	{
		await Task.Delay(100);
    
		if (sender == SalaryWrapper) // optional: name your entries
		{
			await MainScrollView.ScrollToAsync(SalaryWrapper, ScrollToPosition.Center, true);
		}
		else if (sender == LocationWrapper)
		{
			await MainScrollView.ScrollToAsync(LocationWrapper, ScrollToPosition.Center, true);
		}
		else if (sender is VisualElement ve)
		{
			await MainScrollView.ScrollToAsync(ve, ScrollToPosition.Center, true);
		}
	}
}