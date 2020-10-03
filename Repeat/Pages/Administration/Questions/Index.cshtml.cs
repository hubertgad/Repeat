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
        [BindProperty] public int? SelectedCategoryID { get; set; }
        [BindProperty] public int? SelectedSetID { get; set; }

        public async Task OnGetAsync()
        {
            this.Questions = await _questionService.GetQuestionListAsync(this.SelectedCategoryID, this.SelectedSetID);

            ViewData["CategoryID"] = new SelectList(await _questionService.GetCategoryListAsync(), "ID", "Name");
            ViewData["SetID"] = new SelectList(await _questionService.GetSetListAsync(), "ID", "Name");
        }

        public Task OnPostFilterAsync()
        {
            return OnGetAsync();
        }
    }
}