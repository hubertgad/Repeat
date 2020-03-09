using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.DataAccess.Services;
using Repeat.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class ShareModel : CustomPageModelV2
    {
        public ShareModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Share Share { get; set; }
        [BindProperty] public List<Set> Sets { get; set; }
        public List<IdentityUser> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["SetID"] = new SelectList(await _qService.GetSetListAsync(this.CurrentUserID), "ID", "Name");
            ViewData["UserID"] = new SelectList(await _qService.GetUserListAsync(this.CurrentUserID), "Id", "UserName");

            this.Sets = await _qService.GetSetListAsync(this.CurrentUserID);
            this.Users = await _qService.GetUserListAsync(this.CurrentUserID);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!_qService.ShareExists(this.Share))
            {
                await _qService.CreateShareAsync(this.Share);
            }
            else
            {
                return RedirectToPage();
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostUnshareAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_qService.ShareExists(this.Share))
            {
                await _qService.RemoveShareAsync(this.Share);
            }
            else
            {
                return NotFound();
            }

            return RedirectToPage();
        }
    }
}