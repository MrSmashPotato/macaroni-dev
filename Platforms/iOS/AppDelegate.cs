#if IOS || MACCATALYST
using UIKit;
using Foundation;
using Microsoft.Maui.Authentication;
#endif
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;

namespace macaroni_dev;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
	public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
	{
		if (WebAuthenticator.Default != null)
		{
			var uri = new Uri(url.AbsoluteString);
			WebAuthenticator.Default.AuthenticateAsync(new WebAuthenticatorOptions { CallbackUrl = uri });
			return true;
		}
		return false;
	}

	public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
	{
		if (userActivity.WebPageUrl != null && WebAuthenticator.Default != null)
		{
			var uri = new Uri(userActivity.WebPageUrl.AbsoluteString);
			WebAuthenticator.Default.AuthenticateAsync(new WebAuthenticatorOptions { CallbackUrl = uri });
			return true;
		}
		return false;
	}
}
