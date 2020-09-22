using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public DeleteModel(IQuestionService questionService)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Question = await _questionService.GetQuestionByIdAsync(id);
            
            try
            {
                await _questionService.RemoveQuestionAsync(this.Question);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}