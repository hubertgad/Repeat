using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastucture.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Services
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
            model.OwnerId = _currentUserId;
            await _context.Categories.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public Task RemoveCategoryAsync(Category model)
        {
            if (model.OwnerId != _currentUserId) throw new AccessDeniedException();

            _context.Categories.Remove(model);

            return _context.SaveChangesAsync();
        }

        public Task UpdateCategoryAsync(Category model)
        {
            model.OwnerId = _currentUserId;
            _context.Categories.Update(model);

            return _context.SaveChangesAsync();
        }

        public Task<Category> GetCategoryByIdAsync(int id)
        {
            return _context.Categories
                .Where(q => q.OwnerId == _currentUserId && q.Id == id)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync();
        }

        public Task<List<Category>> GetCategoriesForCurrentUserAsync()
        {
            return _context.Categories
                .Where(q => q.OwnerId == _currentUserId)
                .ToListAsync();
        }
    }
}