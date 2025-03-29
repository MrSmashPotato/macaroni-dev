using Android.App;
using Android.Content.PM;
using Microsoft.Maui.Authentication;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace macaroni_dev;

[Activity(NoHistory = true, Exported = true, LaunchMode = LaunchMode.SingleTop)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
    Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable },
    DataScheme = "jobilis")]
public class WebAuthenticatorActivity : WebAuthenticatorCallbackActivity
{
}