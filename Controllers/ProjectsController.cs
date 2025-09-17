using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillNet.Data;
using SkillNet.Hubs;
using SkillNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillNet.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ProjectHub> _projectHub;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHubContext<ProjectHub> projectHub)
        {
            _context = context;
            _userManager = userManager;
            _projectHub = projectHub;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 12)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                .Where(p => p.CreatedById == userId)
                .OrderByDescending(p => p.CreatedDate);

            var paginatedProjects = await PaginatedList<Project>.CreateAsync(
                query.AsNoTracking(), pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            return View(paginatedProjects);
        }

        public IActionResult Create()
        {
            ViewBag.Skills = _context.Skills.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project, int[] selectedSkills)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                project.CreatedById = userId;

                _context.Add(project);
                await _context.SaveChangesAsync();

                if (selectedSkills != null)
                {
                    foreach (var skillId in selectedSkills)
                    {
                        var projectSkill = new ProjectSkill
                        {
                            ProjectId = project.Id,
                            SkillId = skillId
                        };
                        _context.Add(projectSkill);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Skills = _context.Skills.ToList();
            return View(project);
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (project.CreatedById != userId)
            {
                return Forbid();
            }

            ViewBag.Skills = _context.Skills.ToList();
            ViewBag.SelectedSkills = project.ProjectSkills.Select(ps => ps.SkillId).ToArray();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project, int[] selectedSkills)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var existingProject = await _context.Projects
                .Include(p => p.ProjectSkills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProject == null)
            {
                return NotFound();
            }

            if (existingProject.CreatedById != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update project properties
                    existingProject.Title = project.Title;
                    existingProject.Description = project.Description;
                    existingProject.ProjectType = project.ProjectType;
                    existingProject.LookingFor = project.LookingFor;
                    existingProject.EstimatedDuration = project.EstimatedDuration;
                    existingProject.CompensationType = project.CompensationType;
                    existingProject.CompensationDetails = project.CompensationDetails;

                    // Update project skills
                    var currentSkills = existingProject.ProjectSkills.ToList();
                    foreach (var skill in currentSkills)
                    {
                        _context.ProjectSkills.Remove(skill);
                    }

                    if (selectedSkills != null)
                    {
                        foreach (var skillId in selectedSkills)
                        {
                            var projectSkill = new ProjectSkill
                            {
                                ProjectId = existingProject.Id,
                                SkillId = skillId
                            };
                            _context.ProjectSkills.Add(projectSkill);
                        }
                    }

                    _context.Update(existingProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(existingProject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Skills = _context.Skills.ToList();
            ViewBag.SelectedSkills = selectedSkills;
            return View(project);
        }

        public async Task<IActionResult> Browse(int pageIndex = 1, int pageSize = 12, string category = null, string search = null)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                .Where(p => p.CreatedById != userId) // Exclude user's own projects
                .OrderByDescending(p => p.CreatedDate);

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = (IOrderedQueryable<Project>)query.Where(p => p.Title.Contains(search) || 
                                         p.Description.Contains(search) ||
                                         p.LookingFor.Contains(search));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                query = (IOrderedQueryable<Project>)query.Where(p => p.ProjectType == category);
            }

            var paginatedProjects = await PaginatedList<Project>.CreateAsync(
                query.AsNoTracking(), pageIndex, pageSize);

            // Get user's existing interests
            var userInterests = await _context.ProjectInterests
                .Where(pi => pi.UserId == userId)
                .Select(pi => pi.ProjectId)
                .ToListAsync();

            ViewBag.UserInterests = userInterests;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;
            ViewBag.Categories = new List<string> { "Startup", "Side Project", "Freelance", "Open Source", "Non-Profit", "Enterprise" };

            return View(paginatedProjects);
        }

        [HttpPost]
        public async Task<IActionResult> SendInterest(int projectId, string message)
        {
            var userId = _userManager.GetUserId(User);
            
            // Check if project exists
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            
            if (project == null)
            {
                return NotFound();
            }

            // Check if user already sent interest
            var existingInterest = await _context.ProjectInterests
                .FirstOrDefaultAsync(pi => pi.UserId == userId && pi.ProjectId == projectId);

            if (existingInterest != null)
            {
                return BadRequest("You have already expressed interest in this project.");
            }

            // Create new project interest
            var interest = new ProjectInterest
            {
                UserId = userId,
                ProjectId = projectId,
                Message = message,
                Status = InterestStatus.Pending
            };

            _context.ProjectInterests.Add(interest);
            await _context.SaveChangesAsync();

            // Send real-time notification to project owner
            var interestedUser = await _userManager.FindByIdAsync(userId);
            await _projectHub.Clients.Group($"User_{project.CreatedById}").SendAsync("ProjectInterestReceived", new
            {
                interestId = interest.Id,
                projectTitle = project.Title,
                projectId = projectId,
                userName = interestedUser.DisplayName ?? interestedUser.UserName,
                userId = userId,
                message = "Someone is interested in your project!"
            });

            return Ok(new { message = "Your interest has been sent successfully!" });
        }


        [HttpGet]
        public async Task<IActionResult> ProjectInterests(int id, int pageIndex = 1, int pageSize = 10)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.CreatedById == userId);

            if (project == null)
            {
                return NotFound();
            }

            var query = _context.ProjectInterests
                .Include(pi => pi.User)
                .Include(pi => pi.Project)
                .Where(pi => pi.ProjectId == id)
                .OrderByDescending(pi => pi.CreatedDate);

            var paginatedInterests = await PaginatedList<ProjectInterest>.CreateAsync(
                query.AsNoTracking(), pageIndex, pageSize);

            ViewBag.Project = project;
            ViewBag.PageSize = pageSize;
            return View(paginatedInterests);
        }

        [HttpPost]
        public async Task<IActionResult> RespondToInterest(int interestId, string action, string responseMessage = null)
        {
            var userId = _userManager.GetUserId(User);
            var interest = await _context.ProjectInterests
                .Include(pi => pi.Project)
                .Include(pi => pi.User)
                .FirstOrDefaultAsync(pi => pi.Id == interestId && pi.Project.CreatedById == userId);

            if (interest == null)
            {
                return NotFound();
            }

            // Update interest status
            if (action == "accept")
            {
                interest.Status = InterestStatus.Accepted;
            }
            else if (action == "reject")
            {
                interest.Status = InterestStatus.Rejected;
            }
            else
            {
                return BadRequest("Invalid action");
            }

            interest.ResponseDate = DateTime.Now;
            interest.ResponseMessage = responseMessage;

            await _context.SaveChangesAsync();

            // Send real-time notification to the interested user
            var projectOwner = await _userManager.FindByIdAsync(userId);
            await _projectHub.Clients.Group($"User_{interest.UserId}").SendAsync("ProjectInterestResponse", new
            {
                interestId = interest.Id,
                projectTitle = interest.Project.Title,
                projectId = interest.ProjectId,
                ownerName = projectOwner.DisplayName ?? projectOwner.UserName,
                ownerId = userId,
                status = interest.Status.ToString(),
                responseMessage = responseMessage,
                message = $"Your interest in '{interest.Project.Title}' has been {interest.Status.ToString().ToLower()}!"
            });

            return Ok(new { message = "Response sent successfully!" });
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}