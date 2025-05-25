using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;

namespace macaroni_dev.ViewModels;

public partial class EditProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private string _firstName = String.Empty;
    [ObservableProperty]
    private string _middleName = String.Empty;
    [ObservableProperty]
    private string _lastName = String.Empty; 
    [ObservableProperty]
    private string _location = String.Empty;
    [ObservableProperty]
    private string _company = String.Empty;
    [ObservableProperty]
    private string _occupation = String.Empty;
    [ObservableProperty]
    private string _profileImage = String.Empty;
    [ObservableProperty]
    private User _user;

    [ObservableProperty] private bool _isChanging = false;
    public EditProfileViewModel()
    {
        var user = ServiceHelper.GetService<ProfileService>().CurrentUser;
        User = user;
        LastName = user.LastName;
        MiddleName = user.MiddleName;
        FirstName = user.FirstName;
        ProfileImage = user.ProfileImage;
        Company = user.Company;
        Occupation = user.Occupation;
        Location = user.Location;
    }

    partial void OnFirstNameChanged(string value)
    {
        if (value != _user.FirstName)
        {
            IsChanging = true;
        }
    }

    partial void OnLastNameChanged(string value)
    {
        if (value != _user.LastName)
        {
            IsChanging = true;
        }
    }

    partial void OnMiddleNameChanged(string value)
    {
        if (value != _user.MiddleName)
        {
            IsChanging = true;
        }
    }

    partial void OnLocationChanged(string value)
    {
        if (value != _user.Location)
        {
            IsChanging = true;
        }
    }

    partial void OnCompanyChanged(string value)
    {
        if (value != _user.Company)
        {
            IsChanging = true;
        }
    }

    partial void OnProfileImageChanged(string value)
    {
        if (value != _user.ProfileImage)
        {
            IsChanging = true;
        }
    }

    [RelayCommand]
    private async Task Update()
    {
       var res = await  Validate();
       if (res == false)
       {
           await Application.Current.MainPage.DisplayAlert("Update Error", "No Field is changed from the initial values", "OK");
       }
       else
       {
           var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
           User.FirstName = FirstName;
           User.MiddleName = MiddleName;
           User.LastName = LastName;
           User.Company = Company;
           User.Occupation = Occupation;
           User.ProfileImage = ProfileImage;
           User.Location = Location;
           await User.Update<User>();
           await Application.Current.MainPage.DisplayAlert("Update Success", "Changed values are now updated", "OK");
           await Application.Current.MainPage.Navigation.PopAsync();
       }
    }

    private async Task<bool> Validate()
    {
        if (User.FirstName == FirstName &&
            User.MiddleName == MiddleName &&
            User.LastName == LastName &&
            User.Location == Location &&
            User.Company == Company &&
            User.ProfileImage == ProfileImage &&
            User.Occupation == Occupation
           )
        {
            return false;
        } 
        return true;
    }
    partial void OnOccupationChanged(string value)
    {
        if (value != _user.Occupation)
        {
            IsChanging = true;
        }
    }
    

    [RelayCommand]
    private async Task Clear(string par)
    {
        if (par == "FirstName")
        {
            Console.WriteLine("Clearing first name");
            FirstName = String.Empty;
            Console.WriteLine("Cleared Complete: " + User.FirstName);
        } else if (par == "MiddleName")
        {
            MiddleName = string.Empty;
        } else if (par == "LastName")
        {
            LastName = string.Empty;
        }
        else if (par == "Location")
        {
            Location = string.Empty;
        }
        else if (par == "Company")
        {
            Company = string.Empty;
        }
        else if (par == "Occupation")
        {
            Occupation = string.Empty;
        }
        else if (par == "ProfileImage")
        {
            ProfileImage = string.Empty;
        }
        else
        {
            Console.WriteLine("Error");
        }
    }
}