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
    public class DetailsModel : CustomPageModel
    {
        public DetailsModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        public Set Set { get; set; }

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
    }
}