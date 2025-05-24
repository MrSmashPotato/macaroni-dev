using Android.Graphics;
using Android.Widget;
using macaroni_dev.Services;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;

namespace macaroni_dev;

using AColor = Android.Graphics.Color;

public partial class SearchBarExHandler : SearchBarHandler
{
    public void SetIconColor(AColor value)
    {
        var searchIcon = (ImageView)PlatformView.FindViewById(Resource.Id.search_mag_icon);
        searchIcon.SetColorFilter(value, PorterDuff.Mode.SrcAtop);
    }

    public AColor GetTextColor() => VirtualView.TextColor.ToAndroid();

    public static void MapIconColor(ISearchBarHandler handler, ISearchBar searchBar)
    {
        if (handler is SearchBarExHandler customHandler)
            customHandler.SetIconColor(customHandler.GetTextColor());
    }
}