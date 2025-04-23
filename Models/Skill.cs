using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace macaroni_dev.Models
{
    [Table("Skill")]
    public class Skill : BaseModel
    {
        [PrimaryKey("SkillID", false)]
        public int SkillID { get; set; }
        [Column("SkillName")]
        public string SkillName { get; set; } = string.Empty;
        [Column ("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [Column ("IsArchived")]
        public bool IsArchived { get; set; }  
    }
}
