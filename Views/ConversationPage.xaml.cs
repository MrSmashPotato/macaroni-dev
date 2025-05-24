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
    private readonly IKeyboardService _keyboardService;

    public ConversationPage(User user)
    {
        InitializeComponent();
        vm = new ConversationViewModel(user);
        reciever = user;
        ChatLayout.Editor.Focused += (s, e) => AdjustChatLayoutMargin(true);
        ChatLayout.Editor.Unfocused += (s, e) => AdjustChatLayoutMargin(false);
        vm.AdjustLayoutMargin = UnfocusView;
        BindingContext = vm;
#if ANDROID
        _keyboardService = ServiceHelper.GetService<IKeyboardService>();
        _keyboardService.KeyboardVisibilityChanged += OnKeyboardVisibilityChanged;
#endif
    }
    private void UnfocusView()
    {
        ChatLayout.Editor.Unfocus();

    }
    private void OnKeyboardVisibilityChanged(object sender, bool isVisible)
    {
        Console.WriteLine($"Keyboard is visible: {isVisible}");

        if (!isVisible)
        {
            // Optionally unfocus ChatLayout when keyboard is hidden
            ChatLayout.Editor.Unfocus(); // or ChatLayout.Editor.Unfocus() if available
        }
    }
    
    private async void AdjustChatLayoutMargin(bool isFocused)
    {
        Console.WriteLine("Focus Value: " + isFocused);
        var targetMargin = isFocused
            ? new Thickness(0, 15, 0, 20)
            : new Thickness(0, 30, 0, 0);

        await Task.Run(() =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ChatLayout.Margin = targetMargin;
                if (isFocused)
                {
#if ANDROID
                    var nativeView = ChatLayout.Editor.Handler?.PlatformView as Android.Views.View;
                    if (nativeView != null)
                    {
                        var inputMethodManager = nativeView.Context?.GetSystemService(Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
                        inputMethodManager?.ShowSoftInput(nativeView, Android.Views.InputMethods.ShowFlags.Implicit);
                    }
#endif
                }
                else
                {
#if ANDROID
                    var context = Android.App.Application.Context;
                    var inputMethodManager = context.GetSystemService(Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;

                    // Get the native Android view
                    var nativeView = ChatLayout.Editor.Handler.PlatformView as Android.Views.View;

                    if (nativeView != null)
                    {
                        inputMethodManager.HideSoftInputFromWindow(nativeView.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
                    }
#endif
                }
            });
        });
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetTabBarIsVisible(this, false);
        var sender = ServiceHelper.GetService<ProfileService>().CurrentUser;
        await vm.Init(sender.ID, reciever.ID);
    }
    
   

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.SetTabBarIsVisible(this, true);
    }
}