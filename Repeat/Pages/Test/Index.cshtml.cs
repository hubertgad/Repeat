using System;
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

namespace Repeat.Pages.Test
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<SetUser> SetUser { get;set; }
        public string CurrentUserID { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUserID = await GetUserIDAsync();
            SetUser = await _context.SetUsers
                .Include(s => s.Set)
                .Include(s => s.User)
                .Where(t => t.UserID == CurrentUserID )
                .ToListAsync();
        }
        private async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;
    }
}
