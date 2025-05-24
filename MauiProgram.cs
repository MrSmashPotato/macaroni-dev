using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using macaroni_dev.Services;
using macaroni_dev.ViewModels;
using Sharpnado.MaterialFrame;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using UraniumUI;
using Xe.AcrylicView;

namespace macaroni_dev;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		// ⬇️ Add custom SearchBar icon color mapping here
		Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("CustomIconColor", (handler, view) =>
		{
#if ANDROID
			var searchView = handler.PlatformView;

			if (searchView != null)
			{
				int searchIconId = searchView.Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
				var searchIcon = searchView.FindViewById<Android.Widget.ImageView>(searchIconId);

				if (searchIcon != null)
				{
					searchIcon.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.SrcIn); // Change color here
				}
			}
#elif IOS
        var searchBar = handler.PlatformView;

        if (searchBar != null)
        {
            searchBar.SearchTextField.LeftView.TintColor = UIKit.UIColor.Red; // Change color here
        }
#endif
		});
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseAcrylicView()  
			.UseUraniumUI()
			.UseUraniumUIMaterial()
			.UseSharpnadoMaterialFrame(loggerEnable: false)
			.ConfigureSyncfusionCore()
			.ConfigureSyncfusionToolkit()
			
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFontAwesomeIconFonts(); 
				fonts.AddMaterialIconFonts();
				fonts.AddMaterialSymbolsFonts(); // 👈 Add this line


			});
		
		var options = new Supabase.SupabaseOptions
		{
			AutoRefreshToken = true,
			AutoConnectRealtime = true,
		};

		var supabaseClient = new Supabase.Client(
			"https://ovbzwunivnrlfcncogve.supabase.co",  
			"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im92Ynp3dW5pdm5ybGZjbmNvZ3ZlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDE3NDI1NTcsImV4cCI6MjA1NzMxODU1N30.HBZLj_ig4WxUGx_Nkouymj6qCRbOBbShTlFbyWsziGE",  // Replace with your Supabase API key
			options
		);
		supabaseClient.InitializeAsync();
		var supabaseClientProvider = new SupabaseClientProvider(supabaseClient);
		var authService = new AuthService(supabaseClient);
		var profileService = new ProfileService(supabaseClient);
		var MessagesPageViewModel = new MessagesPageViewModel();	
		builder.Services.AddSingleton(profileService);
		builder.Services.AddSingleton<Supabase.Client>();
		builder.Services.AddSingleton(supabaseClientProvider);
		builder.Services.AddSingleton(authService);
		builder.Services.AddSingleton(MessagesPageViewModel);

#if ANDROID
		builder.Services.AddSingleton<IKeyboardService, KeyboardService>();
		
#endif
		
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
