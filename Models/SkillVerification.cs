using System;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class SkillVerification
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }

        [Required]
        public VerificationMethod Method { get; set; }

        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

        [StringLength(1000)]
        public string Evidence { get; set; } // Portfolio link, certificate URL, etc.

        [StringLength(500)]
        public string VerifierNotes { get; set; }

        public string VerifierId { get; set; }
        public virtual ApplicationUser Verifier { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? VerifiedDate { get; set; }

        public int? Score { get; set; } // For test-based verifications (0-100)
    }

    public enum VerificationMethod
    {
        Portfolio,
        Test,
        PeerReview,
        Certificate,
        WorkExperience
    }

    public enum VerificationStatus
    {
        Pending,
        Verified,
        Rejected,
        Expired
    }
}