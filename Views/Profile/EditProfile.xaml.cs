using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Services;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views.Profile;

public partial class EditProfile : ContentPage
{
    public EditProfile(ProfileViewModel vm)
    {
        InitializeComponent();
        var r = BindingContext as EditProfileViewModel;
        r.ProfileViewModel = vm;
    }
    private async void OnInputFocused(object sender, FocusEventArgs e)
    {
        await Task.Delay(100);
    
        if (sender == FirstNameEntry) // optional: name your entries
        {
            await MainScrollView.ScrollToAsync(FirstNameWrapper, ScrollToPosition.Center, true);
        }
        else if (sender == FirstNameWrapper)
        {
            await MainScrollView.ScrollToAsync(FirstNameWrapper, ScrollToPosition.Center, true);
        }
        else if (sender is VisualElement ve)
        {
            await MainScrollView.ScrollToAsync(ve, ScrollToPosition.Center, true);
        }
    }
}