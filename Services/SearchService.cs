using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SkillNet.Data;
using SkillNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillNet.Services
{
    public interface ISearchService
    {
        Task<IQueryable<UserProfile>> SearchUsersAsync(SearchFilters filters);
        Task<IQueryable<Project>> SearchProjectsAsync(ProjectSearchFilters filters);
        Task SaveSearchHistoryAsync(string userId, string searchTerm, string searchType, string filters, int resultCount);
        Task<SavedSearch> SaveSearchAsync(string userId, SavedSearch savedSearch);
        Task<List<SearchHistory>> GetSearchHistoryAsync(string userId, int count = 10);
        Task<List<SavedSearch>> GetSavedSearchesAsync(string userId);
        Task<double> CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }

    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<UserProfile>> SearchUsersAsync(SearchFilters filters)
        {
            var query = _context.UserProfiles
                .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
                .AsQueryable();

            // Basic text search
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                query = query.Where(u =>
                    u.FullName.Contains(filters.SearchTerm) ||
                    u.Title.Contains(filters.SearchTerm) ||
                    u.Bio.Contains(filters.SearchTerm) ||
                    u.UserSkills.Any(us => us.Skill.Name.Contains(filters.SearchTerm))
                );
            }

            // Skill filter
            if (!string.IsNullOrEmpty(filters.SkillFilter))
            {
                query = query.Where(u =>
                    u.UserSkills.Any(us => us.Skill.Name == filters.SkillFilter)
                );
            }

            // Availability filter
            if (!string.IsNullOrEmpty(filters.AvailabilityFilter))
            {
                if (filters.AvailabilityFilter == "1-10")
                    query = query.Where(u => u.HoursPerWeek >= 1 && u.HoursPerWeek <= 10);
                else if (filters.AvailabilityFilter == "10-20")
                    query = query.Where(u => u.HoursPerWeek > 10 && u.HoursPerWeek <= 20);
                else if (filters.AvailabilityFilter == "20+")
                    query = query.Where(u => u.HoursPerWeek > 20);
            }

            // Looking for filter
            if (!string.IsNullOrEmpty(filters.LookingForFilter))
            {
                query = query.Where(u => u.LookingFor == filters.LookingForFilter);
            }

            // Verified skills filter
            if (filters.VerifiedSkillsOnly)
            {
                query = query.Where(u => u.UserSkills.Any(us => us.IsVerified));
            }

            // Experience level filter
            if (!string.IsNullOrEmpty(filters.ExperienceLevel))
            {
                query = query.Where(u => u.UserSkills.Any(us => us.ProficiencyLevel == filters.ExperienceLevel));
            }

            return query;
        }

        public async Task<IQueryable<Project>> SearchProjectsAsync(ProjectSearchFilters filters)
        {
            var query = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSkills)
                .ThenInclude(ps => ps.Skill)
                .AsQueryable();

            // Basic text search
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                query = query.Where(p =>
                    p.Title.Contains(filters.SearchTerm) ||
                    p.Description.Contains(filters.SearchTerm) ||
                    p.ProjectSkills.Any(ps => ps.Skill.Name.Contains(filters.SearchTerm))
                );
            }

            // Project type filter
            if (!string.IsNullOrEmpty(filters.ProjectTypeFilter))
            {
                query = query.Where(p => p.ProjectType == filters.ProjectTypeFilter);
            }

            // Compensation type filter
            if (!string.IsNullOrEmpty(filters.CompensationTypeFilter))
            {
                query = query.Where(p => p.CompensationType == filters.CompensationTypeFilter);
            }

            // Duration filter
            if (!string.IsNullOrEmpty(filters.DurationFilter))
            {
                query = query.Where(p => p.EstimatedDuration == filters.DurationFilter);
            }

            return query;
        }

        public async Task SaveSearchHistoryAsync(string userId, string searchTerm, string searchType, string filters, int resultCount)
        {
            var searchHistory = new SearchHistory
            {
                UserId = userId,
                SearchTerm = searchTerm,
                SearchType = searchType,
                Filters = filters,
                ResultCount = resultCount,
                SearchDate = DateTime.Now
            };

            _context.SearchHistories.Add(searchHistory);
            await _context.SaveChangesAsync();

            // Keep only the latest 50 searches per user
            var oldSearches = await _context.SearchHistories
                .Where(sh => sh.UserId == userId)
                .OrderByDescending(sh => sh.SearchDate)
                .Skip(50)
                .ToListAsync();

            if (oldSearches.Any())
            {
                _context.SearchHistories.RemoveRange(oldSearches);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SavedSearch> SaveSearchAsync(string userId, SavedSearch savedSearch)
        {
            savedSearch.UserId = userId;
            savedSearch.CreatedDate = DateTime.Now;
            savedSearch.LastUsed = DateTime.Now;

            _context.SavedSearches.Add(savedSearch);
            await _context.SaveChangesAsync();

            return savedSearch;
        }

        public async Task<List<SearchHistory>> GetSearchHistoryAsync(string userId, int count = 10)
        {
            return await _context.SearchHistories
                .Where(sh => sh.UserId == userId)
                .OrderByDescending(sh => sh.SearchDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<SavedSearch>> GetSavedSearchesAsync(string userId)
        {
            return await _context.SavedSearches
                .Where(ss => ss.UserId == userId)
                .OrderByDescending(ss => ss.LastUsed)
                .ToListAsync();
        }

        public async Task<double> CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of the Earth in kilometers

            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }
    }

    public class SearchFilters
    {
        public string SearchTerm { get; set; }
        public string SkillFilter { get; set; }
        public string AvailabilityFilter { get; set; }
        public string LookingForFilter { get; set; }
        public bool VerifiedSkillsOnly { get; set; }
        public string ExperienceLevel { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? MaxDistance { get; set; }
    }

    public class ProjectSearchFilters
    {
        public string SearchTerm { get; set; }
        public string ProjectTypeFilter { get; set; }
        public string CompensationTypeFilter { get; set; }
        public string DurationFilter { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? MaxDistance { get; set; }
    }
}