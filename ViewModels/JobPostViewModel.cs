using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Services;
using macaroni_dev.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Supabase.Postgrest;

namespace macaroni_dev.ViewModels
{
    public class JobPostViewModel : BindableObject
    {
        public ICommand SubmitCommand { get; }
        public int currentUserId = 0;
        public string JobTitle { get; set; } = string.Empty;
        public string JobDetail { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int selectedSkill { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterSkills();
            }
        }
        
        private ObservableCollection<Skill> _skills;
        public ObservableCollection<Skill> Skills
        {
            get => _skills;
            set
            {
                _skills = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Skill> _filteredSkills;
        public ObservableCollection<Skill> FilteredSkills
        {
            get => _filteredSkills;
            set
            {
                _filteredSkills = value;
                OnPropertyChanged();
            }
        }

        private Skill _selectedSkill;
        public Skill SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                _selectedSkill = value;
                OnPropertyChanged();
                if (_selectedSkill != null)
                {
                    Console.WriteLine($"Selected Skill: {_selectedSkill.SkillName}, ID: {_selectedSkill.SkillID}");
                }
                SearchText = value?.SkillName; // Set the selected skill name in the search box
                IsSuggestionsVisible = false; // Hide suggestions after selection
            }
        }

        private bool _isSuggestionsVisible;
        public bool IsSuggestionsVisible
        {
            get => _isSuggestionsVisible;
            set
            {
                _isSuggestionsVisible = value;
                OnPropertyChanged();
            }
        }
        private string _jobTitleError;
        public string JobTitleError
        {
            get => _jobTitleError;
            set
            {
                _jobTitleError = value;
                OnPropertyChanged();
            }
        }

        private string _jobDetailError;
        public string JobDetailError
        {
            get => _jobDetailError;
            set
            {
                _jobDetailError = value;
                OnPropertyChanged();
            }
        }

        private string _locationError;
        public string LocationError
        {
            get => _locationError;
            set
            {
                _locationError = value;
                OnPropertyChanged();
            }
        }

        private string _salaryError;
        public string SalaryError
        {
            get => _salaryError;
            set
            {
                _salaryError = value;
                OnPropertyChanged();
            }
        }

        private string _skillError;
        public string SkillError
        {
            get => _skillError;
            set
            {
                _skillError = value;
                OnPropertyChanged();
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            // Validate Job Title
            if (string.IsNullOrWhiteSpace(JobTitle))
            {
                JobTitleError = "Job Title is required.";
                isValid = false;
            }
            else
            {
                JobTitleError = string.Empty;
            }

            // Validate Job Detail
            if (string.IsNullOrWhiteSpace(JobDetail))
            {
                JobDetailError = "Job Description is required.";
                isValid = false;
            }
            else
            {
                JobDetailError = string.Empty;
            }

            // Validate Location
            if (string.IsNullOrWhiteSpace(Location))
            {
                LocationError = "Location is required.";
                isValid = false;
            }
            else
            {
                LocationError = string.Empty;
            }

            // Validate Salary
            if (Salary <= 0)
            {
                SalaryError = "Salary must be greater than 0.";
                isValid = false;
            }
            else
            {
                SalaryError = string.Empty;
            }

            // Validate Skill
            if (SelectedSkill == null)
            {
                SkillError = "Please select a skill from the recommended dropdown items.";
                isValid = false;
            }
            else
            {
                SkillError = string.Empty;
            }

            return isValid;
        }
        public JobPostViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
            FetchSkillsFromDatabase();
        }
        private async void FetchSkillsFromDatabase()
        {
            try
            {
                var client = await SupabaseClientProvider.GetClientAsync();
                var response = await client.From<Skill>().Get();

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
                    Skills.Where(skill => skill.SkillName.ToLower().Contains(SearchText.ToLower()))
                );
                IsSuggestionsVisible = FilteredSkills.Count > 0;
            }
        }

        private async void OnSubmit()
        {
            if (!ValidateForm())
            {
                return; // Stop submission if validation fails
            }
            var authService = await AuthService.GetInstanceAsync();
            var userId = await authService.GetCurrentUserIdAsync();

            if (userId == null)
            {
                // Handle the case where the user ID is not found
                await Application.Current.MainPage.DisplayAlert("Error", "User ID not found. Please log in again.", "OK");
                return;
            }
            // Create a new JobPost object
            var jobPost = new JobPost
            {

                UserID = userId.Value,
                JobName = JobTitle,
                JobDetail = JobDetail,
                Location = Location,
                CreatedAt = DateTime.Now,
                IsDone = false,
                IsArchived = false,
                Salary = Salary
            };
            selectedSkill = SelectedSkill.SkillID;
            
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

                // Insert the job post into the "job_posts" table and return the inserted row
                var response = await client
                .From<JobPost>()
                .Insert(jobPost, new QueryOptions { Returning = QueryOptions.ReturnType.Representation });


                if (response.Models.Count > 0)
                {
                    // Fetch the auto-incremented JobId from the response
                    var insertedJobPost = response.Models.First();
                    jobPost.JobId = insertedJobPost.JobId; // Update the JobPost object with the new JobId

                    Console.WriteLine($"Job post saved successfully with JobId: {jobPost.JobId}");

                    // Insert the job skill into the "job_skills" table
                    var jobSkill = new JobSkill
                    {
                        JobID = jobPost.JobId, // Use the ID of the newly created job post
                        SkillID = selectedSkill,
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
