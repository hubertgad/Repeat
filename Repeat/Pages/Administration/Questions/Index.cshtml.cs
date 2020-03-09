using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Models;
using Repeat.DataAccess.Services;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {

        public IndexModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        public List<Question> Questions { get; set; }
        [BindProperty] public int? SelectedCategoryID { get; set; }
        [BindProperty] public int? SelectedSetID { get; set; }

        public async Task OnGetAsync()
        {
            this.Questions = await _qService.GetQuestionListAsync(this.CurrentUserID, this.SelectedCategoryID, this.SelectedSetID);

            ViewData["CategoryID"] = new SelectList(await _qService.GetCategoryListAsync(this.CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new SelectList(await _qService.GetSetListAsync(this.CurrentUserID), "ID", "Name");          
        }

        public async Task<IActionResult> OnPostCategoryAsync()
        {
            await OnGetAsync();
            return Page();
        }
    }
}