using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.ViewModels;

namespace macaroni_dev.Views.Popup;

public partial class UsersListPopup 
{
    MessagesPageViewModel vm = new MessagesPageViewModel();
    public UsersListPopup(MessagesPageViewModel  vm)
    {
        InitializeComponent();
        vm = this.vm;
        BindingContext = vm;
        vm.LoadUserList();
        if (BindingContext is MessagesPageViewModel viewModel)
        {
            viewModel.CloseAutocompleteDropdown = () =>
            {
                UserAutoComplete.IsDropDownOpen = false;
            };
        }
    }
    
}