using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Services;
using Repeat.Models;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class EditModel : CustomPageModelV2
    {
        public EditModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Category = await _qService.GetCategoryByIDAsync((int)id, this.CurrentUserID);

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
                await _qService.EditCategoryAsync(this.Category);
            }
            catch (DbUpdateConcurrencyException)
            {
                NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}