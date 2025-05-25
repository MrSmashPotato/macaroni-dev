using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Models;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views.Profile;

public partial class Applicants : ContentPage
{
    private JobApplicantsViewModel vm;
    ObservableCollection<JobApplication> applicants;
    public Applicants(ObservableCollection<JobApplication> applications, JobApplicantsViewModel vm)
    {
        applicants = applications;
        InitializeComponent();
        this.vm = vm;
        BindingContext = vm;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await vm.LoadMyApplicants(applicants);
    }
    
}