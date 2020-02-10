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

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public List<Question> Questions { get;set; }
        [BindProperty]
        public int SelectedCategory { get; set; }
        [BindProperty]
        public int SelectedSet { get; set; }

        public async Task OnGetAsync()
        {
            BindDataToView();
            var currentUserID = await GetUserIDAsync();
            Questions = await _context
                .Questions
                .Where(q => q.OwnerID == currentUserID)
                .ToListAsync();
        }
        
        public async Task<IActionResult> OnPostCategoryAsync()
        {
            BindDataToView();
            Questions = await FilterQuestionsAsync();
            var currentUserID = await GetUserIDAsync();
            Questions = Questions.Where(q => q.OwnerID == currentUserID).ToList();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            return Page();
        }
        private void BindDataToView()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name");
            ViewData["SetID"] = new SelectList(_context.Sets, "ID", "Name");
        }
        private async Task<List<Question>> FilterQuestionsAsync()
        {
            if (SelectedCategory < 1 && SelectedSet < 1)
            {
                return await _context.Questions.ToListAsync();
            }
            else if (SelectedCategory < 1)
            {
                return await _context.Questions
                    .Where(q => q.QuestionSets.Any(p => p.SetID == SelectedSet)).ToListAsync();
            }
            else if (SelectedSet < 1)
            {
                return await _context.Questions
                    .Where(q => (q.CategoryID == SelectedCategory)).ToListAsync();
            }
            else
            {
                return await _context.Questions
                    .Where(q => (q.CategoryID == SelectedCategory) &&
                           q.QuestionSets.Any(p => p.SetID == SelectedSet))
                    .ToListAsync();
            }
        }
        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}