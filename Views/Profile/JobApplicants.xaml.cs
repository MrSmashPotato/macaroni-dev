using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views.Profile;

public partial class JobApplicants : ContentPage
{
    private JobApplicantsViewModel vm;
    public JobApplicants()
    {
        vm = new JobApplicantsViewModel();
        BindingContext = vm;
        InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await vm.LoadMyPosts();
    }
}