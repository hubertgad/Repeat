using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using Repeat.Models;

namespace Repeat
{
    public class ShareModel : PageModel
    {
        public ShareModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public SetUser SetUser { get; set; }
        public List<SetUser> SetUsers { get; set; }
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["SetID"] = new SelectList(_context.Sets, "ID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "UserName");

            CurrentUserID = await GetUserIDAsync();
            SetUsers = await _context
                .SetUsers
                .Where(q => q.UserID == CurrentUserID)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SetUsers.Add(SetUser);
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
