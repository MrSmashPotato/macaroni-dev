using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace macaroni_dev.Models
{
    [Table("skill")]
    public class Skill : BaseModel
    {
        [PrimaryKey("id", false)]
        public int ID { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column ("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column ("is_archived")]
        public bool IsArchived { get; set; }  
    }
}
