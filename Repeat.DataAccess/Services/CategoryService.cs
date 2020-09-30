using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _currentUserId;

        public CategoryService(IApplicationDbContext context, ICurrentUserService userService)
        {
            _context = context;
            _currentUserId = userService.UserId;
        }

        public async Task AddCategoryAsync(Category model)
        {
            model.OwnerID = _currentUserId;
            await _context.Categories.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public Task RemoveCategoryAsync(Category model)
        {
            _context.Categories.Remove(model);

            return _context.SaveChangesAsync();
        }

        public Task UpdateCategoryAsync(Category model)
        {
            model.OwnerID = _currentUserId;
            _context.Categories.Update(model);

            return _context.SaveChangesAsync();
        }

        public Task<Category> GetCategoryByIdAsync(int? id)
        {
            return _context.Categories
                .Where(q => q.OwnerID == _currentUserId && q.ID == id)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync();
        }

        public Task<List<Category>> GetCategoriesForCurrentUserAsync()
        {
            return _context.Categories
                .Where(q => q.OwnerID == _currentUserId)
                .ToListAsync();
        }
    }
}