using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillNet.Services;
using SkillNet.Models;
using SkillNet.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SkillNet.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _environment;

        public FilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IFileService fileService, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                return BadRequest("File size too large");

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            try
            {
                var filePath = await _fileService.SaveFileAsync(file, "uploads/profile-pictures");

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    _fileService.DeleteFile(user.ProfilePicture);
                }

                user.ProfilePicture = filePath;
                await _userManager.UpdateAsync(user);

                return Ok(new { filePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadProjectFile(int projectId, IFormFile file, string description = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            var userId = _userManager.GetUserId(User);

            // Check if user has access to the project
            var hasAccess = await _context.ProjectCollaborators
                .AnyAsync(pc => pc.ProjectId == projectId && pc.UserId == userId);

            if (!hasAccess)
                return Forbid();

            try
            {
                var filePath = await _fileService.SaveFileAsync(file, "uploads/project-files");

                var fileAttachment = new FileAttachment
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileType = file.ContentType,
                    FileSize = file.Length,
                    ProjectId = projectId,
                    UserId = userId,
                    Description = description,
                    UploadDate = DateTime.Now
                };

                _context.FileAttachments.Add(fileAttachment);
                await _context.SaveChangesAsync();

                return Ok(new {
                    id = fileAttachment.Id,
                    fileName = fileAttachment.FileName,
                    fileSize = _fileService.GetFileSize(fileAttachment.FileSize)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var fileAttachment = await _context.FileAttachments.FindAsync(id);
            if (fileAttachment == null)
                return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, fileAttachment.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, fileAttachment.FileType, fileAttachment.FileName);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var fileAttachment = await _context.FileAttachments.FindAsync(id);
            if (fileAttachment == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            if (fileAttachment.UserId != userId)
                return Forbid();

            try
            {
                _fileService.DeleteFile(fileAttachment.FilePath);
                _context.FileAttachments.Remove(fileAttachment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }
    }
}