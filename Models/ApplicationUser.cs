using Microsoft.AspNetCore.Identity;
using SkillNet.Models;
using System.Collections.Generic;

namespace SkillNet.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Location { get; set; }
        public string ProfilePicture { get; set; }
        
        // Location coordinates for proximity search
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string TimeZone { get; set; }

        // Navigation properties
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<SkillEndorsement> GivenEndorsements { get; set; }
        public virtual ICollection<SkillEndorsement> ReceivedEndorsements { get; set; }
        public virtual ICollection<SkillVerification> Verifications { get; set; }
        public virtual ICollection<SearchHistory> SearchHistories { get; set; }
        public virtual ICollection<SavedSearch> SavedSearches { get; set; }
    }
}