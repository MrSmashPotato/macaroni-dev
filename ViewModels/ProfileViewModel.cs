using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Platform;
using Supabase.Gotrue;
using User = macaroni_dev.Models.User;

namespace macaroni_dev.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {

        private User _profile;
        [ObservableProperty]
        private string _profileImageUrl;

        [ObservableProperty] private string _name;
        [ObservableProperty] private string _location;
        [ObservableProperty] private string _occupation;
        public ProfileViewModel()
        {
            _profile = ServiceHelper.GetService<ProfileService>().CurrentUser;
            ProfileImageUrl = _profile.ProfileImage;
            var fullName = $"{_profile.FirstName} {_profile.MiddleName} {_profile.LastName}".Trim();
            Name = string.IsNullOrWhiteSpace(fullName) ? "Unspecified" : fullName;

            Occupation = string.IsNullOrWhiteSpace(_profile.Occupation) || string.IsNullOrWhiteSpace(_profile.Company)
                ? "Unspecified"
                : $"{_profile.Occupation} at {_profile.Company}";

            Location = string.IsNullOrWhiteSpace(_profile.Location) ? "Unspecified" : _profile.Location;

            Console.WriteLine("ProfileViewModel");
        }

        [RelayCommand]
        private async void PostJob()
        {
            // Navigate to the JobPost page
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.JobPost());
        }

        [RelayCommand]
        private async void ManageJobs()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.ManageJobs());
        }

        [RelayCommand]
        private async void Applicants()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.JobApplicants());
        }

        [RelayCommand]
        private async void UploadResume()
        {
            
        }

        [RelayCommand]
        private async Task Edit()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.EditProfile(this));
        }
    }
}
