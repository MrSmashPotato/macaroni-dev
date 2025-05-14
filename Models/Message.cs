using System.Runtime.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models;

[Table("message")]
public class Message : BaseModel
{
    [PrimaryKey("id", false)]
    public int ID { get; set; }
    [Column("receiver_id")]
    public Guid ReceiverId { get; set; }
    [Column("sender_id")]
    public Guid SenderId { get; set; }
    [Column("message")]
    public string Value { get; set; }
    
    [Column("created_at",ignoreOnInsert:true,ignoreOnUpdate:true)]
    public DateTime? CreatedAt { get; set; }
    [Column("is_read")]
    public bool IsRead { get; set; }
    [Column("is_archived")]
    public bool IsArchived { get; set; }
}