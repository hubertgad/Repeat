using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;

namespace Repeat
{
    [Authorize]
    public class DetachModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetachModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public QuestionSet QuestionSet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuestionSet = await _context.QuestionSets
                .Include(q => q.Question)
                .Include(q => q.Set).FirstOrDefaultAsync(m => m.QuestionID == id);

            if (QuestionSet == null)
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

            QuestionSet = await _context.QuestionSets.FindAsync(id);

            if (QuestionSet != null)
            {
                _context.QuestionSets.Remove(QuestionSet);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
