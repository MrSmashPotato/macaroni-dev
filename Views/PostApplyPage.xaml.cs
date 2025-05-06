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

    public PostApplyPage(JobPost post, Skill skill, HomePageViewModel homePageViewModel)
    {
        InitializeComponent();
        
        this.BindingContext = new PostApplyViewModel(post,skill,homePageViewModel);
    }
}