using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        public readonly string userId;

        public DetailsModel(ICategoryService categoryService, ICurrentUserService userService)
        {
            _categoryService = categoryService;
            userId = userService.UserId;
        }

        public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.Category = await _categoryService.GetCategoryByIdAsync(id);

            if (this.Category == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}