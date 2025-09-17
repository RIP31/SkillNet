using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillNet.Data;
using SkillNet.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SkillNet.Controllers
{
    [Authorize]
    public class ProjectCollaborationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectCollaborationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> InviteToProject(int projectId, string userId, CollaboratorRole role = CollaboratorRole.Member)
        {
            var currentUserId = _userManager.GetUserId(User);

            var project = await _context.Projects
                .Include(p => p.Collaborators)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            // Check if user has permission to invite
            var isOwnerOrAdmin = project.Collaborators.Any(c =>
                c.UserId == currentUserId &&
                (c.Role == CollaboratorRole.Owner || c.Role == CollaboratorRole.Admin));

            if (!isOwnerOrAdmin)
            {
                return Forbid();
            }

            // Check if user is already a collaborator
            var existingCollaborator = project.Collaborators.FirstOrDefault(c => c.UserId == userId);
            if (existingCollaborator != null)
            {
                return BadRequest("User is already a collaborator");
            }

            var collaborator = new ProjectCollaborator
            {
                ProjectId = projectId,
                UserId = userId,
                Role = role,
                JoinedDate = DateTime.Now
            };

            _context.ProjectCollaborators.Add(collaborator);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCollaborator(int projectId, string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var project = await _context.Projects
                .Include(p => p.Collaborators)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            // Check if user has permission to remove
            var isOwnerOrAdmin = project.Collaborators.Any(c =>
                c.UserId == currentUserId &&
                (c.Role == CollaboratorRole.Owner || c.Role == CollaboratorRole.Admin));

            if (!isOwnerOrAdmin)
            {
                return Forbid();
            }

            var collaborator = project.Collaborators.FirstOrDefault(c => c.UserId == userId);
            if (collaborator == null)
            {
                return NotFound("Collaborator not found");
            }

            _context.ProjectCollaborators.Remove(collaborator);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCollaboratorRole(int projectId, string userId, CollaboratorRole newRole)
        {
            var currentUserId = _userManager.GetUserId(User);

            var project = await _context.Projects
                .Include(p => p.Collaborators)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound("Project not found");
            }

            // Only owner can update roles
            var isOwner = project.Collaborators.Any(c =>
                c.UserId == currentUserId && c.Role == CollaboratorRole.Owner);

            if (!isOwner)
            {
                return Forbid();
            }

            var collaborator = project.Collaborators.FirstOrDefault(c => c.UserId == userId);
            if (collaborator == null)
            {
                return NotFound("Collaborator not found");
            }

            collaborator.Role = newRole;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}