using System;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class SkillEndorsement
    {
        public int Id { get; set; }

        [Required]
        public string EndorserId { get; set; }
        public virtual ApplicationUser Endorser { get; set; }

        [Required]
        public string EndorsedUserId { get; set; }
        public virtual ApplicationUser EndorsedUser { get; set; }

        [Required]
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}