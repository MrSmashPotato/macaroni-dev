using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace macaroni_dev.ViewModels
{
    public class ProfileViewModel : BindableObject
    {
        public ICommand PostJobCommand { get; }

        public ProfileViewModel()
        {
            PostJobCommand = new Command(OnPostJob);
        }

        private async void OnPostJob()
        {
            // Navigate to the JobPost page
            await Application.Current.MainPage.Navigation.PushAsync(new Views.Profile.JobPost());
        }
    }
}
