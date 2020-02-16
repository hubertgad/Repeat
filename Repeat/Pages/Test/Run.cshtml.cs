using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repeat.Data;
using Repeat.Models;

namespace Repeat.Pages.Test
{
    public class RunModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RunModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string CurrentUserID { get; set; }
        public List<Question> Questions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            CurrentUserID = await GetUserIDAsync();
            if (id == null || !CheckAccess(id))
            {
                return NotFound();
            }

            Questions = await _context
                .Questions
                .Include(a => a.Answers)
                .Include(p => p.Picture)
                .Where(m => m.QuestionSets.FirstOrDefault().SetID == id)
                .ToListAsync();

            if (Questions == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage();
        }

        private async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
        private bool CheckAccess(int? id) => _context.SetUsers.Where(q => q.SetID == id).Any(q => q.UserID == CurrentUserID);
    }
}
