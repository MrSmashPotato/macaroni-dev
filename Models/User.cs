
using System;
using System.Runtime.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models
{
    [Table ("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid ID { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("email")]
        public string EmailAddress { get; set; } = "";
        [Column("first_name")]
        public string FirstName { get; set; } = "";

        [Column("middle_name")]
        public string MiddleName { get; set; } = "";

        [Column("last_name")]
        public string LastName { get; set; } = "";
        [Column("is_archived")]
        public bool IsArchived { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("is_complete")]
        public bool IsComplete { get; set; }
        [Column("location")]
        public string Location { get; set; } = "";
        [Column("occupation")]
        public string Occupation { get; set; } = "";
        [Column("company")]
        public string Company { get; set; } = "";
        
        [Column("profile_image")]
        public string ProfileImage { get; set; } = "";
        [IgnoreDataMember]
        public JobApplication? Application { get; set; }
        [IgnoreDataMember]
        public Message LastMessage { get; set; }
    }
}
