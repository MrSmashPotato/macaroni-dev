using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Models;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views.Popup;

public partial class EditJobApplicationPopup
{
    private EditJobApplicationViewModel vm;
    public EditJobApplicationPopup(User user, JobApplicantsViewModel val, JobPost jobPost)
    {
        vm = new EditJobApplicationViewModel(user, val, jobPost);
        BindingContext = vm;
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Mopups.Services.MopupService.Instance.PopAsync();
    }
}