using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<Category> Categories { get;set; }

        public async Task OnGetAsync()
        {
            this.Categories = await _categoryService.GetCategoriesForCurrentUserAsync();
        }
    }
}