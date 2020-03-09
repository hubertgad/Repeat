using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.DataAccess.Services;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class DeleteModel : CustomPageModel
    {
        public DeleteModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Set Set { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Set = await _qService.GetSetByIDAsync((int)id, this.CurrentUserID);

            if (Set != null)
            {
                await _qService.RemoveSetAsync(this.Set);
            }

            return RedirectToPage("./Index");
        }
    }
}