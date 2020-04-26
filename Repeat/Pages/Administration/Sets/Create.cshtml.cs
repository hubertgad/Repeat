using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.DataAccess.Services;
using Repeat.Domain.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class CreateModel : CustomPageModel
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

            this.Set.Shares = new HashSet<Share>
            {
                new Share
                {
                    SetID = this.Set.ID,
                    UserID = this.CurrentUserID
                }
            };

            await _qService.AddAsync(this.Set);

            return RedirectToPage("./Index");
        }
    }
}