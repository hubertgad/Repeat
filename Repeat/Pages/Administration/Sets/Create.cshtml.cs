using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Data;
using Repeat.Models;

namespace Repeat
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
        public Set Set { get; set; }
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
        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}
