using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using macaroni_dev.Services;

namespace macaroni_dev.Views;

public partial class ForgotPage : ContentPage
{
    public ForgotPage()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        if (entryVal.Text == "") return;
        
        
        
        var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        client.Auth.ResetPasswordForEmail(entryVal.Text);
    }
}