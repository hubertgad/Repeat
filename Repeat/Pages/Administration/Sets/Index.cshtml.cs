using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class IndexModel : CustomPageModel
    {
        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
            : base(context, userManager)
        {
        }

        public IList<Set> Set { get;set; }

        public async Task OnGetAsync()
        {
            this.CurrentUserID = await GetUserIDAsync();
            Set = await _context
                .Sets
                .Where(o => o.OwnerID == this.CurrentUserID)
                .ToListAsync();
        }
    }
}