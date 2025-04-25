using CommunityToolkit.Mvvm.ComponentModel;
using macaroni_dev.Models;

namespace macaroni_dev.Services;

public partial class ProfileService : ObservableObject
{
    private readonly Supabase.Client _client;
    [ObservableProperty]
    private User _currentUser;

    public ProfileService(Supabase.Client client)
    {
        _client = client;
    }
    public async Task InitializeProfileAsync(string id)
    {
        var response = await _client.From<User>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Get();

        if (response.Models.Count == 0 || response.Model == null)
        {
            CurrentUser = null;
        }
        else
        {
            CurrentUser = response.Model;
        }
    }
    
}