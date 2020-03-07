using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DetailsModel : CustomPageModel
    {
        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public Question Question { get; set; }
        public Category Category { get; set; }
        public List<Set> Sets { get; set; }
        public FileUpload FileUpload { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            this.CurrentUserID = await GetUserIDAsync();
            this.Question = await GetQuestionFromDatabase(id);
            this.Sets = await GetSetsFromDatabaseAsync();
            
            if (this.Question == null)
            {
                return NotFound();
            }

            return Page();
        }

        private async Task<List<Set>> GetSetsFromDatabaseAsync()
        {
            return await _context.Sets.Where(q => q.QuestionSets.Any(p => p.QuestionID == this.Question.ID)).ToListAsync();
        }

        private async Task<Question> GetQuestionFromDatabase(int? id)
        {
            return await _context.Questions
                            .Include(c => c.Category)
                            .Include(a => a.Answers)
                            .Include(p => p.Picture)
                            .FirstOrDefaultAsync(m => m.ID == id);
        }
    }
}