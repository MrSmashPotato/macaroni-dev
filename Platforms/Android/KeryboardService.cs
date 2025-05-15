using macaroni_dev.Services;

namespace macaroni_dev;
using Android.App;
using Android.Views;
using Microsoft.Maui.Platform;

public class KeyboardService : Java.Lang.Object, IKeyboardService
{
    public bool IsKeyboardVisible { get; private set; }

    public event EventHandler<bool> KeyboardVisibilityChanged;

    public KeyboardService()
    {
        var activity = Platform.CurrentActivity;
        var rootView = activity?.Window?.DecorView?.RootView;

        if (rootView is not null)
        {
            rootView.ViewTreeObserver.GlobalLayout += (s, e) =>
            {
                var r = new Android.Graphics.Rect();
                rootView.GetWindowVisibleDisplayFrame(r);

                int screenHeight = rootView.Height;
                int keypadHeight = screenHeight - r.Bottom;

                bool isVisible = keypadHeight > screenHeight * 0.15;

                if (IsKeyboardVisible != isVisible)
                {
                    IsKeyboardVisible = isVisible;
                    KeyboardVisibilityChanged?.Invoke(this, IsKeyboardVisible);
                }
            };
        }
    }
}