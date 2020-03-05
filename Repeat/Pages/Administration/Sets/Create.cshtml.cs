using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.Data;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class CreateModel : CustomPageModel
    {
        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        [BindProperty] public Set Set { get; set; }

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

            _context.Sets.Add(Set);

            await _context.SaveChangesAsync();

            _context.SetUsers.Add(new SetUser 
            { 
                SetID = this.Set.ID, 
                UserID = await GetUserIDAsync() 
            });

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}