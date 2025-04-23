using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace macaroni_dev.Models
{
    public class Skill
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }  
    }
}
