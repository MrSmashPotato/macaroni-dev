using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Supabase.Postgrest;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace macaroni_dev.ViewModels
{
    public partial class JobPostViewModel : ObservableObject
    {
        [ObservableProperty] private string _jobTitle = string.Empty;
        [ObservableProperty] private string _jobDetail = string.Empty;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private decimal _salary;
        //Selector for ui
        [ObservableProperty] private int _selectedSkill;

        [ObservableProperty] private ObservableCollection<Skill> _skills = new();
        [ObservableProperty] private ObservableCollection<Skill> _filteredSkills = new();
        //Storing the actual selected object
        [ObservableProperty] private Skill _selectedSkillObj = new();
        [ObservableProperty] private bool _isSuggestionsVisible;

        [ObservableProperty] private string _jobTitleError = string.Empty;
        [ObservableProperty] private string _jobDetailError  = string.Empty;
        [ObservableProperty] private string _locationError  = string.Empty;
        [ObservableProperty] private string _salaryError  = string.Empty;
        [ObservableProperty] private string _skillError  = string.Empty;

        [ObservableProperty] private string _searchText = string.Empty;

        // This method is automatically called when SearchText changes.
        partial void OnSearchTextChanged(string oldValue, string newValue)
        {
            FilterSkills();
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
        private async Task SubmitAsync()
        {
            if (!ValidateForm()) return;

            var authService = ServiceHelper.GetService<AuthService>();
            var userId = authService.GetCurrentUser();

            if (userId == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "User ID not found. Please log in again.", "OK");
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
                    await Application.Current.MainPage.DisplayAlert("Success", "Job posted successfully!", "OK");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to post the job. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to post the job: {ex.Message}", "OK");
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