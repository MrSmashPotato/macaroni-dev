using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views.Profile;

public partial class ManageJobs : ContentPage
{
    private ManageJobsViewmodel viewmodel;
    public ManageJobs()
    {
        InitializeComponent();
        viewmodel = new ManageJobsViewmodel();
        BindingContext = viewmodel;
    }
    

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        viewmodel.IsEditEnabled = true;
        await viewmodel.LoadMyPosts();
    }
}