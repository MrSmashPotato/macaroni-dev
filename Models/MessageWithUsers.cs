using System.Text.Json.Serialization;

public class MessageWithUsers
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("sender_id")]
    public string SenderId { get; set; }

    [JsonPropertyName("receiver_id")]
    public string ReceiverId { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("sender_full_name")]
    public string SenderFullName { get; set; }

    [JsonPropertyName("sender_profile_image")]
    public string SenderProfileImage { get; set; }

    [JsonPropertyName("receiver_full_name")]
    public string ReceiverFullName { get; set; }

    [JsonPropertyName("receiver_profile_image")]
    public string ReceiverProfileImage { get; set; }
    [JsonPropertyName("is_read")]
    public bool IsRead { get; set; }
}