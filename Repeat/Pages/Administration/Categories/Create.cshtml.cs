using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.DataAccess.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public Category Category { get; set; }
        [BindProperty]
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUserID = await GetUserIDAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Categories.Add(this.Category);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        private async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
    }
}
