using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    class CategoryService : ICategoryService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _userId;

        public CategoryService(IApplicationDbContext context, ICurrentUserService user)
        {
            _context = context;
            _userId = user.UserId;
        }

        public Task AddCategoryAsync(Category model)
        {
            model.OwnerID = _userId;
            _context.Categories.Add(model);

            return _context.SaveChangesAsync();
        }

        public Task RemoveCategoryAsync(Category model)
        {
            _context.Categories.Remove(model);

            return _context.SaveChangesAsync();
        }

        public Task UpdateCategoryAsync(Category model)
        {
            model.OwnerID = _userId;
            _context.Categories.Update(model);

            return _context.SaveChangesAsync();
        }

        public Task<Category> GetCategoryByIdAsync(int? id)
        {
            return _context.Categories
                .Where(q => q.OwnerID == _userId)
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.ID == id);
        }

        public Task<List<Category>> GetCategoriesForCurrentUserAsync()
        {
            return _context.Categories
                .Where(q => q.OwnerID == _userId)
                .ToListAsync();
        }
    }
}