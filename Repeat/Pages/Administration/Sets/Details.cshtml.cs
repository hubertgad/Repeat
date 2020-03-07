using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class DetailsModel : CustomPageModel
    {
        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public Set Set { get; set; }
        public List<Question> Questions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.CurrentUserID = await GetUserIDAsync();

            this.Set = await _context.Sets.FirstOrDefaultAsync(m => m.ID == id);

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
    }
}