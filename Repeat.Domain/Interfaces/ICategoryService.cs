using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Domain.Interfaces
{
    public interface ICategoryService
    {
        Task AddCategoryAsync(Category model);
        Task RemoveCategoryAsync(Category model);
        Task UpdateCategoryAsync(Category model);
        Task<Category> GetCategoryByIdAsync(int? id);
        Task<List<Category>> GetCategoriesForCurrentUserAsync();
    }
}