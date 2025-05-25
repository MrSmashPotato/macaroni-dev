using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Models;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views;

public partial class PostApplyPage : ContentPage
{
    public PostApplyPage()
    {
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AnimateThrob(ApplyNowButton);
    }
    public PostApplyPage(JobPost post, Skill skill, HomePageViewModel homePageViewModel)
    {
        InitializeComponent();
        
        this.BindingContext = new PostApplyViewModel(post,skill,homePageViewModel);
    }
    private async void AnimateThrob(View view)
    {
        while (true)
        {
            await view.ScaleTo(1.05, 500, Easing.SinInOut);
            await view.ScaleTo(1.0, 500, Easing.SinInOut);
        }
    }
}