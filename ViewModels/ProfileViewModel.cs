using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace macaroni_dev.ViewModels
{
    public partial class ProfileViewModel : BindableObject
    {

        public ProfileViewModel()
        {
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
            
        }

        [RelayCommand]
        private async void Applicants()
        {
            
        }

        [RelayCommand]
        private async void UploadResume()
        {
            
        }
    }
}
