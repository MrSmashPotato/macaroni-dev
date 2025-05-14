using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using macaroni_dev.Models;
using macaroni_dev.Services;
using macaroni_dev.Views;
using Supabase.Postgrest;

namespace macaroni_dev.ViewModels;

public partial class MessagesPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<User> _users = new ObservableCollection<User>();
    private HashSet<Guid> _queriedUserIds = new HashSet<Guid>(); 
    private bool NoMoreItems = false;

    public MessagesPageViewModel()
    {
        Initialize();
    }   
    
    public async Task Initialize()
    {
       await LoadMore();
    }
    
    [RelayCommand]
private async Task LoadMore()
{
    if (NoMoreItems)
        return;

    try
    {
        var server = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        var currentUserId = ServiceHelper.GetService<ProfileService>().CurrentUser.ID;
        Console.WriteLine($"Fetching messages for user {currentUserId}");
        // Step 1: Fetch recent messages involving the current user
        var messages = await server
            .From<Message>()
            .Where(x => x.ReceiverId == currentUserId || x.SenderId == currentUserId)
            .Order(x => x.CreatedAt, Constants.Ordering.Descending)
            .Limit(50) // You can paginate messages this way
            .Get();

        var messageList = messages.Models;
        Console.WriteLine($"Fetched {messageList.Count} messages.");

        if (!messageList.Any())
        {
            Console.WriteLine("No messages found.");
            NoMoreItems = true;
            return;
        }

        // Step 2: Extract other user IDs from messages
        var userMessageDict = new Dictionary<Guid, Message>(); 
        foreach (var message in messageList)
        {
            
            Guid otherUserId = message.SenderId == currentUserId 
                ? message.ReceiverId 
                : message.SenderId;
            
            if (!_queriedUserIds.Contains(otherUserId) && !userMessageDict.ContainsKey(otherUserId))
            {
                userMessageDict[otherUserId] = message;
            }
        }

        var newUserIds = userMessageDict.Keys.Take(5).ToList(); // Limit to 5 users

        if (!newUserIds.Any())
        {
            Console.WriteLine("No new users to query.");
            NoMoreItems = true;
            return;
        }

        var users = await server
            .From<User>()
            .Filter("id", Supabase.Postgrest.Constants.Operator.In, newUserIds)
            .Get();

        foreach (var user in users.Models)
        {
            if (!_queriedUserIds.Contains(user.ID))
            {
                user.LastMessage = userMessageDict[user.ID];
                Users.Add(user);
                _queriedUserIds.Add(user.ID);
                Console.WriteLine($"Added user {user.ID}");
            }
        }

        if (newUserIds.Count < 5)
        {
            NoMoreItems = true;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"LoadMore error: {ex.Message}");
    }
}

[RelayCommand]
    private async Task Conversation(User user)
    {
            await  Shell.Current.Navigation.PushAsync(new ConversationPage(user));
    }
    private async Task ShowAlertAsync(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
    
}