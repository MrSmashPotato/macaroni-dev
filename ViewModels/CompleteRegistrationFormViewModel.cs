using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.Views;
using Microsoft.Maui.Controls;
using Mopups.Services;

namespace macaroni_dev.ViewModels
{
    public partial class CompleteRegistrationViewModel : ObservableObject
    {
        private AuthService _authService;
        [ObservableProperty] private string _username;
        private string _emailAddress;
        [ObservableProperty] private string _password;
        [ObservableProperty] private string _firstName;
        [ObservableProperty] private string _middleName;
        [ObservableProperty] private string _lastName;
        [ObservableProperty] private string _location;
        [ObservableProperty] private string _occupation;
        [ObservableProperty] private string _company;
        [ObservableProperty] private ImageSource _profileImage;
        private MemoryStream _memoryStream = new MemoryStream();
        private string _imageFileName;

        public CompleteRegistrationViewModel()
        {
            Console.WriteLine("CompleteRegistrationViewModel");
        }

        [RelayCommand]
        private async Task AddPhoto()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Choose Photo",
                FileTypes = FilePickerFileType.Images
            });
            _imageFileName = result.FileName;
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                await stream.CopyToAsync(_memoryStream);
                _memoryStream.Position = 0; 
                ProfileImage = ImageSource.FromStream(() => new MemoryStream(_memoryStream.ToArray()));            }
        }
        [RelayCommand]
        private async Task Submit()
        {
            try
            {
                var profileService = ServiceHelper.GetService<ProfileService>();
                var currentUserProfile = profileService.CurrentUser; // Assuming this returns a User or your user model
                Console.WriteLine("ID VALUE " + currentUserProfile.ID.ToString());
                if (currentUserProfile == null || currentUserProfile.ID == Guid.Empty)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "User profile not found.", "OK");
                    return;
                }

                // Update the user fields
                currentUserProfile.FirstName = FirstName;
                currentUserProfile.MiddleName = MiddleName;
                currentUserProfile.LastName = LastName;
                currentUserProfile.Location = Location;
                currentUserProfile.Occupation = Occupation;
                currentUserProfile.Company = Company;
                
                var supabaseClient = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
                var model = await supabaseClient
                    .From<User>()
                    .Where(x => x.ID == currentUserProfile.ID)
                    .Single();

                if (model == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No Profile Found.", "OK");
                }
                else
                {
                    model.FirstName = FirstName;
                    model.MiddleName = MiddleName;
                    model.LastName = LastName;
                    // 👉 Upload the image if it's available
                    if (ProfileImage != null) 
                    {
                        byte[] imageBytes = _memoryStream.ToArray();
                        
                        string fileName = $"profile_images/{_imageFileName}"; 

                        var storage = supabaseClient.Storage;
                        var bucket = storage.From("profileimages");

                        await bucket.Upload(imageBytes, fileName, new Supabase.Storage.FileOptions
                        {
                            Upsert = true
                        });

                        string publicUrl = bucket.GetPublicUrl(fileName);
                        model.ProfileImage = publicUrl;
                    }
                    var result = await model.Update<User>();
                    if (result.Model != null)
                    {
                        model.IsComplete = true;
                        await model.Update<User>();
                        await Application.Current.MainPage.DisplayAlert("Success", "Profile updated successfully!", "OK");
                        await MopupService.Instance.PopAsync();

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Profile update failed!", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
