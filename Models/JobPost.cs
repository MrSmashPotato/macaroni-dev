using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using macaroni_dev.Services;
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

        [JsonIgnore]
        private User user = ServiceHelper.GetService<ProfileService>().CurrentUser;
        
        [IgnoreDataMember]
        public bool IsApplied { get; set; } = false;
        [IgnoreDataMember]
        public string ApplyButtonText => IsApplied ? "Applied" : "Apply Now";
        [IgnoreDataMember]
        public Color ApplyButtonColor => IsApplied ? Colors.Gray : Colors.CornflowerBlue;
        [IgnoreDataMember]
        public bool IsNotCurrentUser => UserID != user.ID;
        [IgnoreDataMember]
        public JobApplication? Application { get; set; }

    }
    
}

