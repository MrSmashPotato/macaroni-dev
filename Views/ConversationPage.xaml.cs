using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views;

public partial class ConversationPage : ContentPage
{
    private ConversationViewModel vm;
    private User profile;
    public ConversationPage(User user)
    {
        InitializeComponent();
        vm = new ConversationViewModel(user);
        profile = user;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var other = ServiceHelper.GetService<ProfileService>().CurrentUser;
        await vm.Init(profile.ID, other.ID);
    }
}