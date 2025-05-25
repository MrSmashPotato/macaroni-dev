using System.ComponentModel;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.ViewModels;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.Popup;
using UraniumUI.Material.Controls;
using SelectionChangedEventArgs = Syncfusion.Maui.Inputs.SelectionChangedEventArgs;

namespace macaroni_dev.Views
{
    public partial class HomePage : ContentPage
    {
        private HomePageViewModel vm;
        private SfChip triff;
        public HomePage()
        {
            vm = new HomePageViewModel();
            BindingContext = vm;
            InitializeComponent();
            Chippy.SelectedItem = Default;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.FetchAsync();
            await vm.FetchSkills();
        }

        private async void ButtonBase_OnClicked(object? sender, EventArgs e)
        {
            var sen = sender as SfChip;
            if (sen == triff)
            {
                Console.WriteLine("Same Chip");  
                return;
            }

            if (sen.Text == "Filter by Skills")
            {
               skillpopup.IsOpen = true;
               return;
            }
            else if (sen.Text == "Filter by Location")
            {
                locationpopup.IsOpen = true;
                return;
            }
            else if (sen.Text == "Filter by Salary")
            {
                salarypopup.IsOpen = true;
                return;
            }
            else
            {
                await vm.ChangeFilter(sen.Text);
            }
            
            triff = sen;
        }

        private async void Autocomplete_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.FirstOrDefault() is Skill selectedSkill)
            {
                vm.FilterSkillId = selectedSkill.ID;
                Console.WriteLine("Selected Skill Value: " + selectedSkill.ID);
                await vm.ChangeFilter("Filter by Skills");
                skillpopup.IsOpen = false;
            }
        }

        private async void Button_OnClicked(object? sender, EventArgs e)
        {
            locationpopup.IsOpen = false;
            await vm.ChangeFilter("Filter by Location");
        }

        private async void Sal_Button_OnClicked(object? sender, EventArgs e)
        {
            salarypopup.IsOpen = false;
            await vm.ChangeFilter("Filter by Salary");
        }
        
        private async void Location_OnClicked(object? sender, EventArgs e)
        {
            locationpopup.IsOpen = false;
            await vm.ChangeFilter("No Filter");
            Chippy.SelectedItem = Default;
        }
        private async void Skills_OnClicked(object? sender, EventArgs e)
        {
            skillpopup.IsOpen = false;
            await vm.ChangeFilter("No Filter");
            Chippy.SelectedItem = Default;
        }
        private async void Salary_OnClicked(object? sender, EventArgs e)
        {
            salarypopup.IsOpen = false;
            await vm.ChangeFilter("No Filter");
            Chippy.SelectedItem = Default;
        }
        
    }
    }