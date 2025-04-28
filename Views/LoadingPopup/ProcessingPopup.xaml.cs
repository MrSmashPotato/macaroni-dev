using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace macaroni_dev.Views.LoadingPopup;

public partial class ProcessingPopup 
{
    public ProcessingPopup()
    {
        InitializeComponent();
        ApplyWavingAnimation();
    }
    private async void ApplyWavingAnimation()
    {
        while (true)
        {
            await LoadingLabel.RotateTo(10, 500, Easing.CubicInOut); 
            await LoadingLabel.RotateTo(-10, 500, Easing.CubicInOut); 
        }
    }
}