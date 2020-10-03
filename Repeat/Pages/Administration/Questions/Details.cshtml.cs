using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Threading.Tasks;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public DetailsModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public Question Question { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Question = await _questionService.GetQuestionByIdAsync(id);

            if (this.Question == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}