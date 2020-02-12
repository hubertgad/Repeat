using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using Repeat.Models;

namespace Repeat
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public DeleteModel(Repeat.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public string CurrentUserID { get; set; }
        [BindProperty]
        public Set Set { get; set; }
        public List<Question> Questions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.CurrentUserID = await GetUserIDAsync();
            Set = await _context
                .Sets
                .Where(o => o.OwnerID == CurrentUserID)
                .FirstOrDefaultAsync(m => m.ID == id);
            this.Questions = await _context
                .Questions
                .Where(o => o.QuestionSets.Any(p => p.SetID == this.Set.ID))
                .ToListAsync();
            if (Set == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Set = await _context.Sets.FindAsync(id);

            if (Set != null)
            {
                _context.Sets.Remove(Set);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}
