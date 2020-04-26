using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Repeat.DataAccess.Services;
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {
        public IndexModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        public List<Category> Categories { get;set; }

        public async Task OnGetAsync()
        {
            this.Categories = await _qService.GetCategoryListAsync(this.CurrentUserID);
        }
    }
}