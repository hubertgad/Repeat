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

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public Question Question { get; set; }
        public Category Category { get; set; }
        public List<Set> Sets { get; set; }
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.CurrentUserID = await GetUserIDAsync();
            Question = await _context
                .Questions
                .Where(m => m.OwnerID == CurrentUserID)
                .Include(o => o.Category)
                .Include(n => n.Answers)
                .Include(p => p.Picture)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (Question == null)
            {
                return NotFound();
            }
            Sets = new List<Set>(_context
                .Sets
                .Where(q => q.QuestionSets
                    .Any(p => p.QuestionID == this.Question.ID))
                );
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Question = await _context.Questions.FindAsync(id);
            if (Question != null)
            {
                _context.Questions.Remove(Question);
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
