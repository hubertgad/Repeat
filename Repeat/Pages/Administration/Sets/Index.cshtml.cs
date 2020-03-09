using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Repeat.DataAccess.Services;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {
        public IndexModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        public List<Set> Set { get;set; }

        public async Task OnGetAsync()
        {
            this.Set = await _qService.GetSetListAsync(this.CurrentUserID);
        }
    }
}