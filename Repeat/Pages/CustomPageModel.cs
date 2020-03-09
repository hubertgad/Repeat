using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.DataAccess.Services;
using System.Threading.Tasks;

namespace Repeat.Pages
{
    [Authorize]
    public class CustomPageModel : PageModel
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly QuestionService _qService;

        public string CurrentUserID
        {
            get { return GetUserIDAsync().Result; }
        }

        public CustomPageModel()
        {
        }

        public CustomPageModel(UserManager<IdentityUser> userManager, QuestionService questionService)
        {
            _userManager = userManager;
            _qService = questionService;
        }

        protected async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
    }
}