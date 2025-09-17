using SkillNet.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Project Type")]
        public string ProjectType { get; set; } // Startup, Side Project, Freelance, Open Source

        [Display(Name = "Looking For")]
        public string LookingFor { get; set; } // Skills needed

        [Display(Name = "Estimated Duration")]
        public string EstimatedDuration { get; set; }

        [Display(Name = "Compensation Type")]
        public string CompensationType { get; set; } // Equity, Paid, Volunteer

        [Display(Name = "Compensation Details")]
        public string CompensationDetails { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual ICollection<ProjectSkill> ProjectSkills { get; set; }
        public virtual ICollection<ProjectCollaborator> Collaborators { get; set; }
    }

    public class ProjectSkill
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int SkillId { get; set; }

        public virtual Project Project { get; set; }
        public virtual Skill Skill { get; set; }
    }
}