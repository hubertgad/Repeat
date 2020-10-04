using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public IndexModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public List<Question> Questions { get; set; }
        [BindProperty] public int? SelectedCategoryId { get; set; }
        [BindProperty] public int? SelectedSetId { get; set; }

        public async Task OnGetAsync()
        {
            this.Questions = await _questionService.GetQuestionListAsync(this.SelectedCategoryId, this.SelectedSetId);

            ViewData["CategoryId"] = new SelectList(await _questionService.GetCategoryListAsync(), "Id", "Name");
            ViewData["SetId"] = new SelectList(await _questionService.GetSetListAsync(), "Id", "Name");
        }

        public Task OnPostFilterAsync()
        {
            return OnGetAsync();
        }
    }
}