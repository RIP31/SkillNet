using System;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class ProjectInterest
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        [StringLength(500)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        public InterestStatus Status { get; set; } = InterestStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ResponseDate { get; set; }

        [StringLength(500)]
        public string ResponseMessage { get; set; }

        // Additional fields for better matching
        public double MatchingScore { get; set; } = 0.0;
        public bool IsHighPriority { get; set; } = false;
    }

    public enum InterestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Withdrawn
    }
}