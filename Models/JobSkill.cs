using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace macaroni_dev.Models
{
    [Table ("JobSkill")]
    public class JobSkill : BaseModel
    {

        [PrimaryKey("JobSkillID", false)]
        public int JobSkillID { get; set; }
        [Column("JobID")]
        public int JobID { get; set; }
        [Column("SkillID")]
        public int SkillID { get; set; }
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [Column("IsArchived")]
        public bool IsArchived { get; set; }
    }
}
