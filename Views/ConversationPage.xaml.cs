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
    private User reciever;
    public ConversationPage(User user)
    {
        InitializeComponent();
        vm = new ConversationViewModel(user);
        reciever = user;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var sender = ServiceHelper.GetService<ProfileService>().CurrentUser;
        await vm.Init(sender.ID, reciever.ID);
    }
}