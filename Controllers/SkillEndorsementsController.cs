using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillNet.Data;
using SkillNet.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SkillNet.Controllers
{
    [Authorize]
    public class SkillEndorsementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SkillEndorsementsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Endorse(string endorsedUserId, int skillId, string comment = null)
        {
            var endorserId = _userManager.GetUserId(User);

            if (endorserId == endorsedUserId)
            {
                return BadRequest("You cannot endorse your own skills");
            }

            // Check if endorsement already exists
            var existingEndorsement = await _context.SkillEndorsements
                .FirstOrDefaultAsync(se => se.EndorserId == endorserId && 
                                          se.EndorsedUserId == endorsedUserId && 
                                          se.SkillId == skillId &&
                                          se.IsActive);

            if (existingEndorsement != null)
            {
                return BadRequest("You have already endorsed this skill for this user");
            }

            var endorsement = new SkillEndorsement
            {
                EndorserId = endorserId,
                EndorsedUserId = endorsedUserId,
                SkillId = skillId,
                Comment = comment
            };

            _context.SkillEndorsements.Add(endorsement);

            // Update endorsement count in UserSkill
            var userSkill = await _context.UserSkills
                .Include(us => us.UserProfile)
                .FirstOrDefaultAsync(us => us.UserProfile.Id.ToString() == endorsedUserId && us.SkillId == skillId);

            if (userSkill != null)
            {
                userSkill.EndorsementCount++;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, newCount = userSkill?.EndorsementCount ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveEndorsement(string endorsedUserId, int skillId)
        {
            var endorserId = _userManager.GetUserId(User);

            var endorsement = await _context.SkillEndorsements
                .FirstOrDefaultAsync(se => se.EndorserId == endorserId && 
                                          se.EndorsedUserId == endorsedUserId && 
                                          se.SkillId == skillId &&
                                          se.IsActive);

            if (endorsement == null)
            {
                return NotFound("Endorsement not found");
            }

            endorsement.IsActive = false;

            // Update endorsement count in UserSkill
            var userSkill = await _context.UserSkills
                .Include(us => us.UserProfile)
                .FirstOrDefaultAsync(us => us.UserProfile.Id.ToString() == endorsedUserId && us.SkillId == skillId);

            if (userSkill != null && userSkill.EndorsementCount > 0)
            {
                userSkill.EndorsementCount--;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, newCount = userSkill?.EndorsementCount ?? 0 });
        }

        [HttpGet]
        public async Task<IActionResult> GetEndorsements(string userId, int skillId)
        {
            var endorsements = await _context.SkillEndorsements
                .Include(se => se.Endorser)
                .Include(se => se.Skill)
                .Where(se => se.EndorsedUserId == userId && se.SkillId == skillId && se.IsActive)
                .OrderByDescending(se => se.CreatedDate)
                .Select(se => new
                {
                    endorserName = se.Endorser.DisplayName,
                    endorserProfilePicture = se.Endorser.ProfilePicture,
                    comment = se.Comment,
                    createdDate = se.CreatedDate.ToString("MMM yyyy")
                })
                .ToListAsync();

            return Json(endorsements);
        }

        [HttpGet]
        public async Task<IActionResult> UserEndorsements(string userId)
        {
            var endorsements = await _context.SkillEndorsements
                .Include(se => se.Endorser)
                .Include(se => se.Skill)
                .Where(se => se.EndorsedUserId == userId && se.IsActive)
                .GroupBy(se => new { se.SkillId, se.Skill.Name, se.Skill.Category, se.Skill.Icon })
                .Select(g => new
                {
                    skillId = g.Key.SkillId,
                    skillName = g.Key.Name,
                    skillCategory = g.Key.Category,
                    skillIcon = g.Key.Icon,
                    count = g.Count(),
                    endorsements = g.OrderByDescending(e => e.CreatedDate).Take(3).Select(e => new
                    {
                        endorserName = e.Endorser.DisplayName,
                        endorserProfilePicture = e.Endorser.ProfilePicture,
                        comment = e.Comment,
                        createdDate = e.CreatedDate.ToString("MMM yyyy")
                    })
                })
                .OrderByDescending(g => g.count)
                .ToListAsync();

            return View(endorsements);
        }
    }
}