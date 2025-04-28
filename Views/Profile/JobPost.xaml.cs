using macaroni_dev.ViewModels;

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
		CreateLabel.IsVisible = true;
		EditLabel.IsVisible = false;
		BindingContext = new JobPostViewModel();
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
}