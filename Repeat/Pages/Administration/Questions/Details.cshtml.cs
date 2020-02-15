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
    public class DetailsModel : PageModel
    {
        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public Question Question { get; set; }
        public Category Category { get; set; }
        public List<Set> Sets { get; set; }
        [BindProperty]
        public FileUpload FileUpload { get; set; }
        public string CurrentUserID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            this.CurrentUserID = await GetUserIDAsync();
            this.Question = await _context.Questions
                .Include(c => c.Category)
                .Include(a => a.Answers)
                .Include(p => p.Picture)
                .FirstOrDefaultAsync(m => m.ID == id);
            Sets = new List<Set>(_context.Sets.Where(q => q.QuestionSets.Any(p => p.QuestionID == this.Question.ID)));
            if (this.Question == null)
            {
                return NotFound();
            }
            return Page();
        }

        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}