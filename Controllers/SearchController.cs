using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SkillNet.Data;
using SkillNet.Models;
using SkillNet.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace SkillNet.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISearchService _searchService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchController(ApplicationDbContext context, ISearchService searchService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _searchService = searchService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchTerm, string skillFilter, string availabilityFilter,
     string lookingForFilter, string experienceLevel, bool verifiedSkillsOnly = false,
     double? maxDistance = null, int pageIndex = 1, int pageSize = 12)
        {
            var currentUserId = _userManager.GetUserId(User);
            var filters = new SearchFilters
            {
                SearchTerm = searchTerm,
                SkillFilter = skillFilter,
                AvailabilityFilter = availabilityFilter,
                LookingForFilter = lookingForFilter,
                ExperienceLevel = experienceLevel,
                VerifiedSkillsOnly = verifiedSkillsOnly,
                MaxDistance = maxDistance
            };

            // Get current user's location for proximity search
            if (User.Identity.IsAuthenticated && maxDistance.HasValue)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Latitude.HasValue == true && currentUser?.Longitude.HasValue == true)
                {
                    filters.Latitude = currentUser.Latitude;
                    filters.Longitude = currentUser.Longitude;
                }
            }

            var query = await _searchService.SearchUsersAsync(filters);
            // Exclude current user from search results
            query = query.Where(u => u.ApplicationUserId != currentUserId);
            var paginatedUsers = await PaginatedList<UserProfile>.CreateAsync(
                query.AsNoTracking(), pageIndex, pageSize);

            // Save search history for authenticated users
            if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(searchTerm))
            {
                var userId = _userManager.GetUserId(User);
                var filtersJson = JsonConvert.SerializeObject(filters);
                await _searchService.SaveSearchHistoryAsync(userId, searchTerm, "Users", filtersJson, paginatedUsers.TotalCount);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SkillFilter = skillFilter;
            ViewBag.AvailabilityFilter = availabilityFilter;
            ViewBag.LookingForFilter = lookingForFilter;
            ViewBag.ExperienceLevel = experienceLevel;
            ViewBag.VerifiedSkillsOnly = verifiedSkillsOnly;
            ViewBag.MaxDistance = maxDistance;
            ViewBag.Skills = await _context.Skills.Where(s => s.IsActive).OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync();
            ViewBag.SkillCategories = await _context.Skills.Where(s => s.IsActive).Select(s => s.Category).Distinct().OrderBy(c => c).ToListAsync();
            ViewBag.PageSize = pageSize;

            return View(paginatedUsers);
        }

        public async Task<IActionResult> Projects(string searchTerm, string projectTypeFilter, 
            string compensationTypeFilter, string durationFilter, double? maxDistance = null)
        {
            var filters = new ProjectSearchFilters
            {
                SearchTerm = searchTerm,
                ProjectTypeFilter = projectTypeFilter,
                CompensationTypeFilter = compensationTypeFilter,
                DurationFilter = durationFilter,
                MaxDistance = maxDistance
            };

            // Get current user's location for proximity search
            if (User.Identity.IsAuthenticated && maxDistance.HasValue)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Latitude.HasValue == true && currentUser?.Longitude.HasValue == true)
                {
                    filters.Latitude = currentUser.Latitude;
                    filters.Longitude = currentUser.Longitude;
                }
            }

            var query = await _searchService.SearchProjectsAsync(filters);
            var results = await query.OrderByDescending(p => p.CreatedDate).ToListAsync();

            // Save search history for authenticated users
            if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(searchTerm))
            {
                var userId = _userManager.GetUserId(User);
                var filtersJson = JsonConvert.SerializeObject(filters);
                await _searchService.SaveSearchHistoryAsync(userId, searchTerm, "Projects", filtersJson, results.Count);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.ProjectTypeFilter = projectTypeFilter;
            ViewBag.CompensationTypeFilter = compensationTypeFilter;
            ViewBag.DurationFilter = durationFilter;
            ViewBag.MaxDistance = maxDistance;

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = _userManager.GetUserId(User);
            var searchHistory = await _searchService.GetSearchHistoryAsync(userId, 20);

            return View(searchHistory);
        }

        [HttpGet]
        public async Task<IActionResult> SavedSearches()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = _userManager.GetUserId(User);
            var savedSearches = await _searchService.GetSavedSearchesAsync(userId);

            return View(savedSearches);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSearch(string name, string searchTerm, string searchType, string filters)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { success = false, message = "Authentication required" });

            var userId = _userManager.GetUserId(User);
            var savedSearch = new SavedSearch
            {
                Name = name,
                SearchTerm = searchTerm,
                SearchType = searchType,
                Filters = filters
            };

            await _searchService.SaveSearchAsync(userId, savedSearch);
            return Json(new { success = true });
        }

        [Authorize]
        public async Task<IActionResult> FindTalent(string searchTerm, string skillFilter, string availabilityFilter,
            string lookingForFilter, string experienceLevel, bool verifiedSkillsOnly = false, 
            double? maxDistance = null, int pageIndex = 1, int pageSize = 12)
        {
            var currentUserId = _userManager.GetUserId(User);
            var filters = new SearchFilters
            {
                SearchTerm = searchTerm,
                SkillFilter = skillFilter,
                AvailabilityFilter = availabilityFilter,
                LookingForFilter = lookingForFilter,
                ExperienceLevel = experienceLevel,
                VerifiedSkillsOnly = verifiedSkillsOnly,
                MaxDistance = maxDistance
            };

            // Get current user's location for proximity search
            if (maxDistance.HasValue)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Latitude.HasValue == true && currentUser?.Longitude.HasValue == true)
                {
                    filters.Latitude = currentUser.Latitude;
                    filters.Longitude = currentUser.Longitude;
                }
            }

            var query = await _searchService.SearchUsersAsync(filters);
            var paginatedUsers = await PaginatedList<UserProfile>.CreateAsync(
                query.AsNoTracking(), pageIndex, pageSize);


            // Save search history
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var filtersJson = Newtonsoft.Json.JsonConvert.SerializeObject(filters);
                await _searchService.SaveSearchHistoryAsync(currentUserId, searchTerm, "Talent", filtersJson, paginatedUsers.TotalCount);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SkillFilter = skillFilter;
            ViewBag.AvailabilityFilter = availabilityFilter;
            ViewBag.LookingForFilter = lookingForFilter;
            ViewBag.ExperienceLevel = experienceLevel;
            ViewBag.VerifiedSkillsOnly = verifiedSkillsOnly;
            ViewBag.MaxDistance = maxDistance;
            ViewBag.Skills = await _context.Skills.Where(s => s.IsActive).OrderBy(s => s.Category).ThenBy(s => s.Name).ToListAsync();
            ViewBag.SkillCategories = await _context.Skills.Where(s => s.IsActive).Select(s => s.Category).Distinct().OrderBy(c => c).ToListAsync();
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentUserId = currentUserId;

            return View(paginatedUsers);
        }

    }
}