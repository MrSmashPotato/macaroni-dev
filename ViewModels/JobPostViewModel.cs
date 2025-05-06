using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Supabase.Postgrest;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Views;
using macaroni_dev.Views.LoadingPopup;
using Mopups.Services;

namespace macaroni_dev.ViewModels
{
    public partial class JobPostViewModel : ObservableObject
    {
        [ObservableProperty] private string _jobTitle = string.Empty;
        [ObservableProperty] private string _jobDetail = string.Empty;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private bool _isButtonEnabled = true;
        [ObservableProperty] private decimal _salary;
        
        //Selector for ui
        [ObservableProperty] private int _selectedSkill;

        [ObservableProperty] private ObservableCollection<Skill> _skills = new();
        [ObservableProperty] private ObservableCollection<Skill> _filteredSkills = new();
        //Storing the actual selected object
        [ObservableProperty] private Skill _selectedSkillObj;
        [ObservableProperty] private bool _isSuggestionsVisible;

        [ObservableProperty] private string _jobTitleError = string.Empty;
        [ObservableProperty] private string _jobDetailError  = string.Empty;
        [ObservableProperty] private string _locationError  = string.Empty;
        [ObservableProperty] private string _salaryError  = string.Empty;
        [ObservableProperty] private string _skillError  = string.Empty;

        [ObservableProperty] private string _searchText = string.Empty;

        private Skill nonEditedSkill;
        private JobPost nonEditedJobPost;
        
        
        [RelayCommand]
        private async Task ClearTitle()
        {
            JobTitle = string.Empty;
            JobTitleError = string.Empty;
        }

        [RelayCommand]
        private async Task ClearSalary()
        {
            Salary = 0;
            SalaryError = string.Empty;
        }
        [RelayCommand]
        private async Task ClearSkills()
        {
            SearchText = string.Empty;
            SelectedSkillObj = null; // <- clear the selected skill too

            SkillError = string.Empty;
        }
        [RelayCommand]
        private async Task ClearDetail()
        {
            JobDetail = string.Empty;
            JobDetailError = string.Empty;
        }
        [RelayCommand]
        private async Task ClearLocation()
        {
            Location = string.Empty;
            LocationError = string.Empty;
        }
        // This method is automatically called when SearchText changes.
        partial void OnSearchTextChanged(string oldValue, string newValue)
        {
            SelectedSkillObj = null;
            FilterSkills();
        }

        partial void OnJobDetailChanged(string oldValue, string newValue)
        {
            if (newValue == null)
                return;

            string limitedValue = newValue;
            string error = string.Empty;

            var lines = limitedValue.Split('\n');
            int maxLines = 25;
            int maxChars = 1000;
            int consumedPerLine = 40; 

            int consumed = (lines.Length - 1) * consumedPerLine; 

           
            if (limitedValue.Length > 1000)
            {
                limitedValue = limitedValue.Substring(0, 1000);
                error = "Job Description should not exceed 1000 characters.";
            }
            else if (lines.Length > maxLines)
            {
                limitedValue = string.Join('\n', lines.Take(maxLines));
                error = "Job Description should not exceed 25 lines.";
            }
            else if (limitedValue.Length + consumed > maxChars)
            {
                int remainingChars = maxChars - consumed;
                var truncatedLines = limitedValue.Split('\n').Select((line, index) =>
                {
                    if (index == maxLines - 1 && line.Length > remainingChars)
                    {
                        error = "Job Description should not exceed 1000 characters.";
                        return line.Substring(0, remainingChars); // Trim the last line if it exceeds remaining characters
                    }
                    return line;
                }).ToList();
                
                limitedValue = string.Join('\n', truncatedLines);

            }
            if (limitedValue != newValue)
            {
                JobDetail = limitedValue;
            }

            JobDetailError = error; 
        }
        public JobPostViewModel()
        {
            
            try
            {
                FetchSkillsFromDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public JobPostViewModel(JobPost jobPost, Skill skill)
        {
            this.nonEditedJobPost = jobPost;
            this.nonEditedSkill = skill;
            JobTitle = jobPost.JobName;
            JobDetail = jobPost.JobDetail;
            Location = jobPost.Location;
            Salary = jobPost.Salary;
            SelectedSkillObj = skill;
            SearchText = skill.Name;
            try
            {
                FetchSkillsFromDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        // Asynchronous initialization method
        private async void FetchSkillsFromDatabase()
        {
            try
            {
                var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
                var response = await client.From<Skill>().Get();
                Console.WriteLine(response.Models.Count());
                if (response.Models.Count > 0)
                {
                    Skills = new ObservableCollection<Skill>(response.Models);
                    FilteredSkills = new ObservableCollection<Skill>(Skills);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching skills: {ex.Message}");
            }
        }

        private void FilterSkills()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredSkills = new ObservableCollection<Skill>(Skills);
                IsSuggestionsVisible = false;
            }
            else
            {
                FilteredSkills = new ObservableCollection<Skill>(
                    Skills.Where(skill => skill.Name.ToLower().Contains(SearchText.ToLower()))
                );
                IsSuggestionsVisible = FilteredSkills.Count > 0;
            }
        }

        partial void OnSelectedSkillObjChanged(Skill oldValue, Skill newValue)
        {
            if (newValue != null)
            {
                Console.WriteLine($"Selected Skill: {newValue.Name}, ID: {newValue.ID}");
                SearchText = newValue.Name;
                SelectedSkillObj = newValue;
                IsSuggestionsVisible = false;
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(JobTitle))
            {
                JobTitleError = "Job Title is required.";
                isValid = false;
            }
            else JobTitleError = string.Empty;

            if (string.IsNullOrWhiteSpace(JobDetail))
            {
                JobDetailError = "Job Description is required.";
                isValid = false;
            }
            else JobDetailError = string.Empty;

            if (string.IsNullOrWhiteSpace(Location))
            {
                LocationError = "Location is required.";
                isValid = false;
            }
            else LocationError = string.Empty;

            if (Salary <= 0)
            {
                SalaryError = "Salary must be greater than 0.";
                isValid = false;
            }
            else SalaryError = string.Empty;

            if (SelectedSkillObj == null)
            {
                SkillError = "Please select a skill from the recommended dropdown items.";
                isValid = false;
            }
            else SkillError = string.Empty;

            return isValid;
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            IsButtonEnabled = false;
            await MopupService.Instance.PushAsync(new ProcessingPopup());

            bool isConfirmed = await ShowConfirmationAlertAsync("Confirm Deletion", "Are you sure you want to delete this job post?");
            if (!isConfirmed)
            {
                await MopupService.Instance.PopAsync();
                IsButtonEnabled = true;
                return;
            }

            var supabaseClient = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();

            try
            {
                var jobPost = await supabaseClient
                    .From<JobPost>()
                    .Where(x => x.Id == nonEditedJobPost.Id)
                    .Single();

                if (jobPost == null)
                {
                    await MopupService.Instance.PopAsync();
                    await ShowAlertAsync("Error", "The job post could not be found.");
                    IsButtonEnabled = true;
                    return;
                }

                await jobPost.Delete<JobPost>();

                await MopupService.Instance.PopAsync();
                await ShowAlertAsync("Deletion Successful", "The job post has been deleted successfully!");
                IsButtonEnabled = true;
                Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await MopupService.Instance.PopAsync();
                await ShowAlertAsync("Deletion Failed", $"An error occurred: {ex.Message}");
                IsButtonEnabled = true;
            }
        }
        public async Task<bool> ShowConfirmationAlertAsync(string title, string message)
        {
            var result = await Application.Current.MainPage.DisplayAlert(title, message, "Yes", "No");
            return result;  
        }
        [RelayCommand]
        private async Task UpdateAsync()
        {
            IsButtonEnabled = false;
            await MopupService.Instance.PushAsync(new ProcessingPopup());

            if (!ValidateForm())
            {
                await MopupService.Instance.PopAsync();
                IsButtonEnabled = true;
                return;
            }
               
            bool noChanges =
                nonEditedSkill.ID == SelectedSkillObj.ID &&
                nonEditedJobPost.JobName == JobTitle &&
                nonEditedJobPost.JobDetail == JobDetail &&
                nonEditedJobPost.Location == Location &&
                nonEditedJobPost.Salary == Salary;

            if (noChanges)
            {
                await MopupService.Instance.PopAsync();
                IsButtonEnabled = true;
                await ShowAlertAsync("No changes detected.", "Please modify at least one field to update.");
                return;
            }
            
            
            var supabaseClient = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();

            try
            {
                var jobPost = await supabaseClient
                    .From<JobPost>()
                    .Where(x => x.Id == nonEditedJobPost.Id)
                    .Single();

                if (jobPost == null)
                {
                    await MopupService.Instance.PopAsync();
                    await ShowAlertAsync("Error", "The job post could not be found.");
                    IsButtonEnabled = true;
                    return;
                }

                // 2. Update the fields
                jobPost.JobName = JobTitle;
                jobPost.JobDetail = JobDetail;
                jobPost.Location = Location;
                jobPost.Salary = Salary;

                // 3. Push the update
                await jobPost.Update<JobPost>();
                if (nonEditedSkill.ID != SelectedSkillObj.ID)
                {
                    var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
                    if (nonEditedJobPost?.Id == null)
                    {
                        await MopupService.Instance.PopAsync();
                        await ShowAlertAsync("Error", "Job Post ID is missing.");
                        IsButtonEnabled = true;
                        return;
                    }

                    var jobSkill = await client
                        .From<JobSkill>() // <-- now using your real table name
                        .Where(m => m.JobID == nonEditedJobPost.Id)
                        .Single();
                    if (jobSkill != null)
                    {
                        jobSkill.SkillID = SelectedSkillObj.ID;
                        await jobSkill.Update<JobSkill>();
                    }
                }
                await MopupService.Instance.PopAsync();
                await ShowAlertAsync("Update Successful", "The job post has been updated successfully!");
                IsButtonEnabled = true;
                Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await MopupService.Instance.PopAsync();
                await ShowAlertAsync("Update Failed", $"An error occurred: {ex.Message}");
                IsButtonEnabled = true;
            }
        }
        private async Task ShowAlertAsync(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
        
        [RelayCommand]
        private async Task SubmitAsync()
        {
            await MopupService.Instance.PushAsync(new ProcessingPopup());

            if (!ValidateForm())
            {
                await MopupService.Instance.PopAsync();
                IsButtonEnabled = true;
                return;
            }

            var authService = ServiceHelper.GetService<AuthService>();
            var userId = authService.GetCurrentUser();

            if (userId == null)
            {
                await MopupService.Instance.PopAsync();
                await Application.Current.MainPage.DisplayAlert("Error", "User ID not found. Please log in again.", "OK");
                IsButtonEnabled = true;
                return;
            }

            var jobPost = new JobPost
            {
                UserID = Guid.Parse(userId.Id),
                JobName = JobTitle,
                JobDetail = JobDetail,
                Location = Location,
                CreatedAt = DateTime.Now,
                IsDone = false,
                IsArchived = false,
                Salary = Salary
            };

            SelectedSkill = SelectedSkillObj.ID;

            
            try
            {
                bool isSaved = await SaveJobPostToDatabaseAsync(jobPost);

                if (isSaved)
                {
                    await MopupService.Instance.PopAsync();
                    await Application.Current.MainPage.DisplayAlert("Success", "Job posted successfully!", "OK");
                    IsButtonEnabled = true;
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await MopupService.Instance.PopAsync();
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to post the job. Please try again.", "OK");
                    IsButtonEnabled = true;
                }
            }
            catch (Exception ex)
            {
                await MopupService.Instance.PopAsync();
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to post the job: {ex.Message}", "OK");
                IsButtonEnabled = true;
            }
        }

        private async Task<bool> SaveJobPostToDatabaseAsync(JobPost jobPost)
        {
            try
            {
                var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
                var response = await client.From<JobPost>().Insert(jobPost, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

                if (response.Models.Count > 0)
                {
                    var insertedJobPost = response.Models.First();
                    jobPost.Id = insertedJobPost.Id;

                    var jobSkill = new JobSkill
                    {
                        JobID = jobPost.Id,
                        SkillID = SelectedSkill,
                        CreatedAt = DateTime.Now,
                        IsArchived = false
                    };

                    await client.From<JobSkill>().Insert(jobSkill);
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