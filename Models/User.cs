
using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace macaroni_dev.Models
{
    [Table ("User")]
    public class User : BaseModel
    {
        [PrimaryKey("UserID", false)]
        public int UserID { get; set; }
        [Column("Username")]
        public string Username { get; set; }
        [Column("EmailAddress")]
        public string EmailAddress { get; set; } = "";
        [Column("Password")]
        public string Password { get; set; } = "";
        [Column("FirstName")]
        public string FirstName { get; set; } = "";

        [Column("MiddleName")]
        public string MiddleName { get; set; } = "";

        [Column("LastName")]
        public string LastName { get; set; } = "";
        [Column("IsArchived")]
        public bool IsArchived { get; set; }
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}
