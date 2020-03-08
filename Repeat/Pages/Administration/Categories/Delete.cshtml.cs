﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repeat.DataAccess.Services;
using Repeat.Models;

namespace Repeat.Pages.Administration.Categories
{
    [Authorize]
    public class DeleteModel : CustomPageModelV2
    {
        public DeleteModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Category Category { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Category = await _qService.GetCategoryByIDAsync((int)id, this.CurrentUserID);

            if (this.Category == null)
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

            this.Category = await _qService.GetCategoryByIDAsync((int)id, this.CurrentUserID);

            try
            {
                await _qService.MarkCategoryAsDeleted(this.Category);
            }
            catch
            {
                NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}