using System;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class SearchHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [StringLength(500)]
        public string SearchTerm { get; set; }

        public string SearchType { get; set; } // "Users", "Projects"

        public string Filters { get; set; } // JSON string of applied filters

        public DateTime SearchDate { get; set; } = DateTime.Now;

        public int ResultCount { get; set; }
    }

    public class SavedSearch
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string SearchTerm { get; set; }

        public string SearchType { get; set; } // "Users", "Projects"

        public string Filters { get; set; } // JSON string of applied filters

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUsed { get; set; } = DateTime.Now;

        public bool EmailNotifications { get; set; } = false;
        public int NotificationFrequency { get; set; } = 7; // Days
    }
}