using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Threading.Tasks;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public EditModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty] public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Category = await _categoryService.GetCategoryByIdAsync((int) id);

            if (this.Category == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _categoryService.UpdateCategoryAsync(this.Category);
            }
            catch (DbUpdateConcurrencyException)
            {
                NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}