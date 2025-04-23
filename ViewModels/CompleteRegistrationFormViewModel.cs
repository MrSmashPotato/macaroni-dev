using System;
using System.Windows.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Microsoft.Maui.Controls;

namespace macaroni_dev.ViewModels
{
    public class CompleteRegistrationViewModel : BindableObject
    {
        private AuthService _authService;

        private string _username;
        private string _emailAddress;
        private string _password;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

       
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string MiddleName
        {
            get => _middleName;
            set
            {
                _middleName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; }

        public CompleteRegistrationViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
        }

        private async void OnSubmit()
        {
            try
            {
                var authService = await AuthService.GetInstanceAsync();
                var currentUser = authService.GetCurrentUser();

                if (currentUser == null || string.IsNullOrEmpty(currentUser.Email))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No authenticated user found.", "OK");
                    return;
                }
                // Create a new User object with form data
                var newUser = new User
                {
                    Username = Username,
                    EmailAddress = currentUser.Email,
                    Password = Password,
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
                    IsArchived = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Save the user to the database
                var supabaseClient = authService.GetSupabaseClient();
                var result = await supabaseClient.From<User>().Insert(newUser);

                var insertedUser = result.Models.FirstOrDefault();
                if (insertedUser != null)
                {
                    // Save the UserID in SecureStorage
                    await SecureStorage.SetAsync(GlobalVariables.CurrentUserID, insertedUser.UserID.ToString());
                }

                // Navigate to the main application page
                await Application.Current.MainPage.DisplayAlert("Success", "Registration completed successfully!", "OK");
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
