using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class ShareModel : CustomPageModel
    {
        public ShareModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        [BindProperty] public SetUser SetUser { get; set; }
        [BindProperty] public List<Set> Sets { get; set; }
        public List<IdentityUser> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["SetID"] = new SelectList(_context.Sets.Where(q => q.OwnerID == this.CurrentUserID), "ID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users.Where(q => q.Id != this.CurrentUserID), "Id", "UserName");

            this.CurrentUserID = await GetUserIDAsync();
            this.Sets = await GetSetsFromDBAsync();
            this.Users = await _context.Users.ToListAsync();

            return Page();
        }

        private async Task<List<Set>> GetSetsFromDBAsync()
        {
            return await _context
                .Sets
                .Include(q => q.SetUsers)
                .Where(q => q.OwnerID == CurrentUserID)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!_context.SetUsers.Any(q => q == SetUser))
            {
                _context.SetUsers.Add(SetUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToPage();
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUnshareAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.SetUsers.Any(q => q == SetUser))
            {
                _context.SetUsers.Remove(SetUser);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToPage();
        }
    }
}