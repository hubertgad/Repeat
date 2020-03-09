using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Repeat.DataAccess.Services;
using Repeat.Models;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {
        public IndexModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        public List<Set> Sets { get; set; }
        public List<Category> Categories { get; set; }

        public async Task OnGetAsync()
        {
            this.Sets = await _qService.GetSetListAsync(this.CurrentUserID, true);
        }
    }
}