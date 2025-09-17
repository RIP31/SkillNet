using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillNet.Data;
using SkillNet.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SkillNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.UserProfiles
                .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
                .ToListAsync();

            return View(users);
        }

        public IActionResult Create()
        {
            ViewBag.Skills = _context.Skills.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfile userProfile, int[] selectedSkills, string[] proficiencyLevels)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userProfile);
                await _context.SaveChangesAsync();

                if (selectedSkills != null)
                {
                    for (int i = 0; i < selectedSkills.Length; i++)
                    {
                        var userSkill = new UserSkill
                        {
                            UserProfileId = userProfile.Id,
                            SkillId = selectedSkills[i],
                            ProficiencyLevel = proficiencyLevels[i]
                        };
                        _context.Add(userSkill);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Skills = _context.Skills.ToList();
            return View(userProfile);
        }
    }
}