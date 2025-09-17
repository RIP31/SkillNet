using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Professional Title")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        [Display(Name = "Hours Available Per Week")]
        [Range(1, 40)]
        public int HoursPerWeek { get; set; }

        [Display(Name = "Looking For")]
        public string LookingFor { get; set; }

        public virtual ICollection<UserSkill> UserSkills { get; set; }
    }

    public class Skill
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Category { get; set; }

        public string Icon { get; set; } // Font Awesome icon class or URL

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserSkill> UserSkills { get; set; }
        public virtual ICollection<SkillEndorsement> Endorsements { get; set; }
        public virtual ICollection<SkillVerification> Verifications { get; set; }
    }

    public class UserSkill
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public int SkillId { get; set; }
        public string ProficiencyLevel { get; set; } // Beginner, Intermediate, Expert

        public int EndorsementCount { get; set; } = 0;
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedDate { get; set; }
        public int YearsOfExperience { get; set; } = 0;

        public virtual UserProfile UserProfile { get; set; }
        public virtual Skill Skill { get; set; }
    }
}