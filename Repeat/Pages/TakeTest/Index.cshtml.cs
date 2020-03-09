using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {
        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public IList<Share> SetUser { get;set; }

        public async Task OnGetAsync()
        {
            this.CurrentUserID = await GetUserIDAsync();
            this.SetUser = await GetSetUsersFromDatabaseAsync();
        }

        private async Task<IList<Share>> GetSetUsersFromDatabaseAsync()
        {
            return await _context.Shares
                .Include(s => s.Set)
                //.Include(s => s.User)
                .Where(t => t.UserID == CurrentUserID && t.Set.QuestionSets.Any())
                .ToListAsync();
        }
    }
}
