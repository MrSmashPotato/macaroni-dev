using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Services;
using macaroni_dev.Models;
using System.Windows.Input;

namespace macaroni_dev.ViewModels
{
    public class JobPostViewModel : BindableObject
    {
        public ICommand SubmitCommand { get; }
        public int currentUserId = 123;
        public string JobTitle { get; set; } = string.Empty;
        public string JobDetail { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public JobPostViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
        }

        private async void OnSubmit()
        {
            // Create a new JobPost object
            var jobPost = new JobPost
            {
                UserID = currentUserId,
                JobName = JobTitle,
                JobDetail = JobDetail,
                Location = Location,
                CreatedAt = DateTime.Now,
                IsDone = false,
                IsArchived = false
            };

            // Simulate saving the job post (replace with actual database or API logic)
            try
            {
                bool isSaved = await SaveJobPostToDatabaseAsync(jobPost);

                if (isSaved)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Job posted successfully!", "OK");
                    await Application.Current.MainPage.Navigation.PopAsync(); // Navigate back
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to post the job. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Display the actual error message
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to post the job: {ex.Message}", "OK");
            }
        }
        private async Task<bool> SaveJobPostToDatabaseAsync(JobPost jobPost)
        {
            try
            {
                var client = await SupabaseClientProvider.GetClientAsync();

                // Insert the job post into the "job_posts" table
                var response = await client.From<JobPost>().Insert(jobPost);

                if (response.Models.Count > 0)
                {
                    Console.WriteLine("Job post saved successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving job post: {ex.Message}");
            }

            return false;
        }
       
    }
}
