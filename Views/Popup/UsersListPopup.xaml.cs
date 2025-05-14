using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.ViewModels;
using Mopups.Services;

namespace macaroni_dev.Views.Popup;

public partial class UsersListPopup
{
    private MessagesPageViewModel viewm;
    public UsersListPopup(MessagesPageViewModel  vm)
    {
        InitializeComponent();
        viewm = vm;
        BindingContext = viewm;
        vm.LoadUserList();
        if (BindingContext is MessagesPageViewModel viewModel)
        {
            viewModel.CloseAutocompleteDropdown = () =>
            {
                UserAutoComplete.IsDropDownOpen = false;
            };
        }
    }
    
    public void  ClosePopup(Object o, EventArgs e)
    {
        viewm.ButtonEnabled = true; 
        MopupService.Instance.PopAsync();
    }
    
}