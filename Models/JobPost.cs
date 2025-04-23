using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models
{
    [Table("JobPost")] 
    public class JobPost : BaseModel
    {
        [PrimaryKey("JobId", false)]
        public int JobId { get; set; }


        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("JobName")]
        public string JobName { get; set; } = "";

        [Column("JobDetail")]
        public string JobDetail { get; set; } = "";

        [Column("Location")]
        public string Location { get; set; } = "";

        [Column("IsDone")]
        public bool IsDone { get; set; }

        [Column("IsArchived")]
        public bool IsArchived { get; set; }

        [Column("UserID")]
        public int UserID { get; set; }
    }
}

