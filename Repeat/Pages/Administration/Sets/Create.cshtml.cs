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
    public class CreateModel : CustomPageModelV2
    {
        public CreateModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Set Set { get; set; }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            this.Set.Shares = new List<Share>
            {
                new Share
                {
                    SetID = this.Set.ID,
                    UserID = this.CurrentUserID
                }
            };

            await _qService.CreateSetAsync(this.Set);

            return RedirectToPage("./Index");
        }
    }
}