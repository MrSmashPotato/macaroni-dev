﻿using macaroni_dev.Services;
using macaroni_dev.Views;
using Supabase.Gotrue;
namespace macaroni_dev;

public partial class App : Application
{
    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzgwODAyM0AzMjM5MmUzMDJlMzAzYjMyMzkzYmNNMHl1RlFMUi8zRzk1UEVqUTlyVnkzVXpwVm1rM2tSeGR4RW1iSG5VbkE9");

        InitializeComponent();
        MainPage = new AppShell();
    }
    protected override async void OnStart()
    {
        base.OnStart();
        var sessionToken = await SecureStorage.GetAsync("session_token");
        var refreshToken = await SecureStorage.GetAsync("refresh_token");
        if (!string.IsNullOrEmpty(sessionToken) && !string.IsNullOrEmpty(refreshToken))
        {
            var authService = ServiceHelper.GetService<AuthService>();
            var user = await authService.RestoreSessionAsync(sessionToken, refreshToken);
            
            if (user != null)
            {
                Console.WriteLine(user.Email);
                MainPage = new AppShell();
                return;
            }
        }
        MainPage = new NavigationPage(new LoginPage());
    }
    
}
