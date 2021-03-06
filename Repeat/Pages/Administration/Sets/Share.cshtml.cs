﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat
{
    [Authorize]
    public class ShareModel : PageModel
    {
        private readonly ISetService _setService;
        public ShareModel(ISetService setService)
        {
            _setService = setService;
        }

        [BindProperty] public Share Share { get; set; }
        public List<Set> Sets { get; set; }
        [BindProperty] public int? SetId { get; set; }
        [BindProperty] public string UserName { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            this.Sets = await _setService.GetSetsForCurrentUserAsync();
            ViewData["SetId"] = new SelectList(this.Sets, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _setService.AddShareAsync((int)this.SetId, this.UserName);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnshareAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _setService.RemoveShareAsync(this.Share);

            return RedirectToPage();
        }
    }
}