using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Data;
using System.Threading.Tasks;

namespace Repeat.Pages
{
    [Authorize]
    public class CustomPageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<IdentityUser> _userManager;
        public string CurrentUserID { get; set; }

        public CustomPageModel()
        {
        }

        public CustomPageModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
    }
}