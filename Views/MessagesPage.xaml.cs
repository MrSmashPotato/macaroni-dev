using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.ViewModels;
using Syncfusion.Maui.DataSource;

namespace macaroni_dev.Views;

public partial class MessagesPage : ContentPage
{
    public MessagesPage()
    {
        InitializeComponent();
        listView.GroupHeaderTemplate = new DataTemplate(() => 
        {
            var grid = new Grid();
            var headerLabel = new Label
            {
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor=Colors.Teal
            };
            headerLabel.SetBinding(Label.TextProperty, new Binding("Key"));
            var dataTrigger = new DataTrigger(typeof(Label))
            {
                Binding = new Binding("Key"),
                Value = "Unread Messages"
            };
            dataTrigger.Setters.Add(new Setter
            {
                Property = Label.BackgroundColorProperty,
                Value = Colors.IndianRed
            });

            // Default background (e.g., for "Read" or empty key)
            headerLabel.BackgroundColor = Colors.Teal;

            headerLabel.Triggers.Add(dataTrigger);

            grid.Children.Add(headerLabel);
            return grid;
        });
        listView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
        {
            PropertyName = "LastMessage.IsRead", // Optional but informative
            KeySelector = (object obj) =>
            {
                var item = obj as User; // Replace with your actual class name
                var cur = ServiceHelper.GetService<ProfileService>().CurrentUser;

                if (item?.LastMessage == null)
                    return "";

                if (item.LastMessage.SenderId == cur.ID)
                {
                    return "";
                }
                

                if (item.LastMessage.IsRead == false)
                    return "Unread Messages";

                return "";
            },
        });
        this.listView.DataSource.SortDescriptors.Add(new SortDescriptor()
        {
            PropertyName = "LastMessage.IsRead",
            Direction = ListSortDirection.Descending
        });
       
      
    }
    
    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var vm = BindingContext as MessagesPageViewModel;
        vm.ButtonEnabled = true;
        await vm.Initialize();
    }
    
    
}