using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models;

[Table("job_post_saved")]
public class JobSaved : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("post_id")]
    public int PostId { get; set; }
    [Column("saved_at")]
    public DateTime SavedAt { get; set; }
}