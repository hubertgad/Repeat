using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.DataAccess.Services;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DeleteModel : CustomPageModel
    {
        public DeleteModel(UserManager<IdentityUser> userManager, QuestionService questionService) 
            : base(userManager, questionService)
        {
        }

        public Question Question { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);

            if (this.Question == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);

            try
            {
                await _qService.MarkQuestionAsDeleted(this.Question);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}