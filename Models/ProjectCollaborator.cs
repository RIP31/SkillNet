using SkillNet.Models;
using System;

namespace SkillNet.Models
{
    public class ProjectCollaborator
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public CollaboratorRole Role { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.Now;

        public virtual Project Project { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public enum CollaboratorRole
    {
        Owner,
        Admin,
        Member,
        Contributor
    }
}