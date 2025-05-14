using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using macaroni_dev.Models;
using macaroni_dev.Services;
using Supabase.Postgrest;
using Syncfusion.Maui.Chat;

using Supabase.Realtime.PostgresChanges;
using Syncfusion.Maui.Chat;

namespace macaroni_dev.ViewModels;

public partial class ConversationViewModel : ObservableObject
{
    [ObservableProperty]
    private Author receiver;
    [ObservableProperty]
    private Author sender;
    public ObservableCollection<object> Messages { get; set; } = new();
    
    public ConversationViewModel(User user)
    {
        var currentUser = ServiceHelper.GetService<ProfileService>().CurrentUser;

        Sender = new Author(){
            Name = currentUser.FirstName + " " + currentUser.MiddleName + " " + currentUser.LastName
            };   
        Receiver = new Author(){
            Name = user.FirstName + " " + user.MiddleName + " " + user.LastName
        };  
        Console.WriteLine("Sender Name: " + Sender.Name);
        //Nykko Fuentes Millanes
        
    }

    private void GenerateMessage(string name, Uri image, DateTime sentat, string text)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Author temp;
            if (name == Sender.Name)
            {
                temp = Sender;
            }
            else
            {
                temp = new Author { Name = name, Avatar = image};
            }
            var message = new TextMessage
            {
                Author = temp,
                DateTime = sentat,
                Text = text
            };

            Messages.Add(message);
    
            // Debugging output
            Console.WriteLine("Message added:");
            Console.WriteLine($"Name: {message.Author?.Name}");
            Console.WriteLine($"Text: {message.Text}");
            Console.WriteLine($"Total Messages: {Messages.Count}");
        });
        Console.WriteLine("Message added");
        Console.WriteLine(Messages.Count);
    }
    
    public async Task Init(Guid currentUserId, Guid otherUserId)
    {
        try
        {
            var loaded = await LoadMessages(currentUserId, otherUserId);

            if (loaded == null || loaded.Count <= 0)
            {
                Console.WriteLine("No messages found");
                await SubscribeToMessages(currentUserId, otherUserId);
                return;
            }

            Console.WriteLine("Messages found");
            foreach (var load in loaded)
            {
                GenerateMessage(
                    load.SenderFullName,
                    new Uri(load.SenderProfileImage),
                    load.CreatedAt,
                    load.Content
                );
            }

            await SubscribeToMessages(currentUserId, otherUserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error initializing messages: " + ex.Message);
        }
    }
    private async Task<List<MessageWithUsers>> LoadMessages(Guid currentUserId, Guid otherUserId)
    {
        try
        {
            Console.WriteLine("Loading messages");
            var client = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();

            var parameters = new Dictionary<string, Guid>
            {
                { "current_user_id", currentUserId },
                { "other_user_id", otherUserId }
            };

            var rawResponse = await client.Rpc("get_messages", parameters);

// This gives you access to the raw JSON string:
            var json = rawResponse.Content.ToString();

            Console.WriteLine("Raw JSON Response:");
            Console.WriteLine(json);

// Optional: Deserialize manually for debugging
            var messages = JsonSerializer.Deserialize<List<MessageWithUsers>>(json);

            if (messages != null && messages.Count > 0)
            {
                Console.WriteLine("Success! First message sender: " + messages[0].SenderFullName);
                return messages;
            }
            else
            {
                Console.WriteLine("Deserialization failed or empty.");
                return new List<MessageWithUsers>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading messages: " + ex.Message);
            return null;
        }
    }
    private async Task SubscribeToMessages(Guid currentUserId, Guid otherUserId)
    {
        var supabase = ServiceHelper.GetService<SupabaseClientProvider>().GetSupabaseClient();
        var channel = supabase.Realtime.Channel();

        await supabase.From<Message>().On(PostgresChangesOptions.ListenType.Inserts, (sender, change) =>
        {
            var entity = change.Model<Message>();
            Console.WriteLine("Entity: " + entity.Value);
            Console.WriteLine("Entity: " + entity.SenderId);

        });
        
        await channel.Subscribe();
    }
}

   
   