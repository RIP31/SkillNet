using SkillNet.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SkillNet.Models
{
    public class FileAttachment
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public string FileType { get; set; }
        public long FileSize { get; set; }

        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;
        public string Description { get; set; }
    }
}