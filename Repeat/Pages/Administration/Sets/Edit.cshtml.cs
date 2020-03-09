using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Services;
using Repeat.Domain.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class EditModel : CustomPageModel
    {
        public EditModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Set Set { get; set; }
        [BindProperty] public QuestionSet QuestionSet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Set = await _qService.GetSetByIDAsync((int)id, this.CurrentUserID);

            if (this.Set == null)
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
                await _qService.EditSetAsync(this.Set);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDetachAsync()
        {
            try
            {
                await _qService.RemoveQuestionFromSetAsync(this.QuestionSet);
            }
            catch
            {
                NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}