using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Threading.Tasks;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DeleteModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty] public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.Category = await _categoryService.GetCategoryByIdAsync(id);

            if (this.Category == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Category category)
        {
            try
            {
                await _categoryService.RemoveCategoryAsync(category);
            }
            catch
            {
                NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}