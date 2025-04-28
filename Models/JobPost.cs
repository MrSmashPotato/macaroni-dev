using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models
{
    [Table("job_post")] 
    public class JobPost : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("name")]
        public string JobName { get; set; } = "";

        [Column("detail")]
        public string JobDetail { get; set; } = "";
    
        [Column("location")]
        public string Location { get; set; } = "";

        [Column("is_done")]
        public bool IsDone { get; set; }

        [Column("is_archived")]
        public bool IsArchived { get; set; }

        [Column("user_id")]
        public Guid UserID { get; set; }
        [Column("salary")]
        public decimal Salary { get; set; }
    }
}

