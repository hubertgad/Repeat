using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.DataAccess.Services;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DetailsModel : CustomPageModelV2
    {
        public DetailsModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        public Question Question { get; set; }
        public FileUpload FileUpload { get; set; }

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
    }
}