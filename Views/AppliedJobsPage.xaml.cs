using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views;

public partial class AppliedJobsPage : ContentPage
{
    private AppliedJobsViewmodel vm;
    public AppliedJobsPage()
    {
        vm = new AppliedJobsViewmodel();
        BindingContext = vm;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await vm.Fetch();
    }
}