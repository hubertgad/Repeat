using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using Repeat.Models;

namespace Repeat
{
    [Authorize]
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
        [BindProperty]
        public List<Set> Sets { get; set; }
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["SetID"] = new SelectList(_context.Sets.Where(q => q.OwnerID == this.CurrentUserID), "ID", "Name");
            ViewData["UserID"] = new SelectList(_context.Users.Where(q => q.Id != this.CurrentUserID), "Id", "UserName");
            CurrentUserID = await GetUserIDAsync();
            Sets = await _context
                .Sets
                .Include(q => q.SetUsers)
                .Where(q => q.OwnerID == CurrentUserID)
                .ToListAsync();
            return Page();
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

        private async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
    }
}