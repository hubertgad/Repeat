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

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {
        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public List<Question> Questions { get; set; }
        [BindProperty] public int SelectedCategoryID { get; set; }
        [BindProperty] public int SelectedSetID { get; set; }

        public async Task OnGetAsync()
        {
            this.CurrentUserID = await GetUserIDAsync();
            BindDataToView();
            this.Questions = await GetQuestionsFromDatabaseAsync();
        }

        private void BindDataToView()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(q => q.OwnerID == CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new SelectList(_context.Sets.Where(q => q.OwnerID == CurrentUserID), "ID", "Name");
        }

        private async Task<List<Question>> GetQuestionsFromDatabaseAsync() => await _context.Questions.Where(q => q.OwnerID == CurrentUserID).ToListAsync();

        public async Task<IActionResult> OnPostCategoryAsync()
        {
            BindDataToView();
            this.CurrentUserID = await GetUserIDAsync();
            this.Questions = await FilterQuestionsAsync();
            return Page();
        }

        private async Task<List<Question>> FilterQuestionsAsync()
        {
            if (SelectedCategoryID < 1 && SelectedSetID < 1)
            {
                return await GetQuestionsFromDatabaseAsync();
            }
            else if (SelectedCategoryID < 1)
            {
                return await _context.Questions
                    .Where(q => q.QuestionSets.Any(p => p.SetID == SelectedSetID) && q.OwnerID == this.CurrentUserID)
                    .ToListAsync();
            }
            else if (SelectedSetID < 1)
            {
                return await _context.Questions
                    .Where(q => q.CategoryID == SelectedCategoryID && q.OwnerID == this.CurrentUserID)
                    .ToListAsync();
            }
            else
            {
                return await _context.Questions
                    .Where(q => (q.CategoryID == SelectedCategoryID) &&
                           q.QuestionSets.Any(p => p.SetID == SelectedSetID) &&
                           q.OwnerID == this.CurrentUserID)
                    .ToListAsync();
            }
        }
    }
}