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
    public class DetachModel : PageModel
    {
        public DetachModel(Repeat.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public QuestionSet QuestionSet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? sid, int? qid)
        {
            if (sid == null || qid == null)
            {
                return NotFound();
            }

            QuestionSet = await _context
                .QuestionSets
                .FirstOrDefaultAsync(m => m.QuestionID == qid && m.SetID == sid);

            if (QuestionSet == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? sid, int? qid)
        {
            if (sid == null || qid == null)
            {
                return NotFound();
            }

            QuestionSet = await _context.QuestionSets
                .FirstOrDefaultAsync(q => q.QuestionID == qid
                && q.SetID == sid);

            if (QuestionSet != null)
            {
                _context.QuestionSets.Remove(QuestionSet);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Edit", new { id = sid });
        }
    }
}
