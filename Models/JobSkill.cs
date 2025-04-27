using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
namespace macaroni_dev.Models
{
    [Table ("job_skill")]
    public class JobSkill : BaseModel
    {

        [PrimaryKey("id", false)]
        public int ID { get; set; }
        [Column("job_id")]
        public int JobID { get; set; }
        [Column("skill_id")]
        public int SkillID { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("is_archived")]
        public bool IsArchived { get; set; }
    }
}
