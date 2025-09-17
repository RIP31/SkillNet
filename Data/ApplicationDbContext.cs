using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillNet.Models;
namespace SkillNet.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectSkill> ProjectSkills { get; set; }
        public DbSet<ProjectCollaborator> ProjectCollaborators { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }
        public DbSet<SkillEndorsement> SkillEndorsements { get; set; }
        public DbSet<SkillVerification> SkillVerifications { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<SavedSearch> SavedSearches { get; set; }
        public DbSet<ProjectInterest> ProjectInterests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This is important for Identity

            // Your existing configuration
            modelBuilder.Entity<UserSkill>()
                .HasKey(us => us.Id);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.UserProfile)
                .WithMany(u => u.UserSkills)
                .HasForeignKey(us => us.UserProfileId);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId);

            modelBuilder.Entity<ProjectSkill>()
                .HasKey(ps => ps.Id);

            modelBuilder.Entity<ProjectSkill>()
                .HasOne(ps => ps.Project)
                .WithMany(p => p.ProjectSkills)
                .HasForeignKey(ps => ps.ProjectId);

            modelBuilder.Entity<ProjectSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany()
                .HasForeignKey(ps => ps.SkillId);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.CreatedBy)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.CreatedById);


            // Project Collaborator configuration
            modelBuilder.Entity<ProjectCollaborator>()
                .HasKey(pc => pc.Id);

            modelBuilder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.Collaborators)
                .HasForeignKey(pc => pc.ProjectId);

            modelBuilder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId);

            // File attachment configuration
            modelBuilder.Entity<FileAttachment>()
                .HasOne(f => f.Project)
                .WithMany()
                .HasForeignKey(f => f.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileAttachment>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Skill Endorsement configuration
            modelBuilder.Entity<SkillEndorsement>()
                .HasOne(se => se.Endorser)
                .WithMany(u => u.GivenEndorsements)
                .HasForeignKey(se => se.EndorserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SkillEndorsement>()
                .HasOne(se => se.EndorsedUser)
                .WithMany(u => u.ReceivedEndorsements)
                .HasForeignKey(se => se.EndorsedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SkillEndorsement>()
                .HasOne(se => se.Skill)
                .WithMany(s => s.Endorsements)
                .HasForeignKey(se => se.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            // Skill Verification configuration
            modelBuilder.Entity<SkillVerification>()
                .HasOne(sv => sv.User)
                .WithMany(u => u.Verifications)
                .HasForeignKey(sv => sv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SkillVerification>()
                .HasOne(sv => sv.Skill)
                .WithMany(s => s.Verifications)
                .HasForeignKey(sv => sv.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SkillVerification>()
                .HasOne(sv => sv.Verifier)
                .WithMany()
                .HasForeignKey(sv => sv.VerifierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Search History configuration
            modelBuilder.Entity<SearchHistory>()
                .HasOne(sh => sh.User)
                .WithMany(u => u.SearchHistories)
                .HasForeignKey(sh => sh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Saved Search configuration
            modelBuilder.Entity<SavedSearch>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.SavedSearches)
                .HasForeignKey(ss => ss.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Project Interest configuration
            modelBuilder.Entity<ProjectInterest>()
                .HasOne(pi => pi.User)
                .WithMany()
                .HasForeignKey(pi => pi.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectInterest>()
                .HasOne(pi => pi.Project)
                .WithMany()
                .HasForeignKey(pi => pi.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectInterest>()
                .HasIndex(pi => new { pi.UserId, pi.ProjectId })
                .IsUnique();

            // Seed initial skills
            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "C#", Category = "Programming", Icon = "fab fa-microsoft", Description = "Microsoft's object-oriented programming language" },
                new Skill { Id = 2, Name = "JavaScript", Category = "Programming", Icon = "fab fa-js", Description = "Dynamic programming language for web development" },
                new Skill { Id = 3, Name = "Python", Category = "Programming", Icon = "fab fa-python", Description = "High-level programming language for various applications" },
                new Skill { Id = 4, Name = "UI/UX Design", Category = "Design", Icon = "fas fa-paint-brush", Description = "User interface and user experience design" },
                new Skill { Id = 5, Name = "Graphic Design", Category = "Design", Icon = "fas fa-palette", Description = "Visual communication and problem-solving through design" },
                new Skill { Id = 6, Name = "React", Category = "Frontend", Icon = "fab fa-react", Description = "JavaScript library for building user interfaces" },
                new Skill { Id = 7, Name = "Vue.js", Category = "Frontend", Icon = "fab fa-vuejs", Description = "Progressive JavaScript framework" },
                new Skill { Id = 8, Name = "Node.js", Category = "Backend", Icon = "fab fa-node-js", Description = "JavaScript runtime for server-side development" },
                new Skill { Id = 9, Name = "SQL", Category = "Database", Icon = "fas fa-database", Description = "Structured Query Language for database management" },
                new Skill { Id = 10, Name = "MongoDB", Category = "Database", Icon = "fas fa-leaf", Description = "NoSQL document-oriented database" },
                new Skill { Id = 11, Name = "Project Management", Category = "Business", Icon = "fas fa-tasks", Description = "Planning and organizing project activities" },
                new Skill { Id = 12, Name = "Digital Marketing", Category = "Business", Icon = "fas fa-bullhorn", Description = "Online marketing and promotion strategies" },
                new Skill { Id = 13, Name = "Content Writing", Category = "Business", Icon = "fas fa-pen", Description = "Creating engaging written content" },
                new Skill { Id = 14, Name = "Java", Category = "Programming", Icon = "fab fa-java", Description = "Object-oriented programming language" },
                new Skill { Id = 15, Name = "Angular", Category = "Frontend", Icon = "fab fa-angular", Description = "TypeScript-based web application framework" },
                new Skill { Id = 16, Name = "Docker", Category = "DevOps", Icon = "fab fa-docker", Description = "Containerization platform" },
                new Skill { Id = 17, Name = "AWS", Category = "Cloud", Icon = "fab fa-aws", Description = "Amazon Web Services cloud platform" },
                new Skill { Id = 18, Name = "Machine Learning", Category = "AI/ML", Icon = "fas fa-robot", Description = "Algorithms that learn from data" },
                new Skill { Id = 19, Name = "Data Analysis", Category = "Data Science", Icon = "fas fa-chart-line", Description = "Analyzing and interpreting data" },
                new Skill { Id = 20, Name = "Cybersecurity", Category = "Security", Icon = "fas fa-shield-alt", Description = "Protecting systems from digital attacks" }
            );
        }
    }
}