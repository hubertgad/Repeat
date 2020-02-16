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
    public class EditModel : PageModel
    {
        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public string CurrentUserID { get; set; }
        [BindProperty]
        public Set Set { get; set; }
        [BindProperty]
        public int QuestionID { get; set; }
        [BindProperty]
        public List<Question> Questions { get; set; }
        [BindProperty]
        public QuestionSet QuestionSet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.CurrentUserID = await GetUserIDAsync();
            Set = await _context
                .Sets
                .Include(q => q.QuestionSets)
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Set).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SetExists(Set.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }
        
        private bool SetExists(int id)
        {
            return _context.Sets.Any(e => e.ID == id);
        }

        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}
