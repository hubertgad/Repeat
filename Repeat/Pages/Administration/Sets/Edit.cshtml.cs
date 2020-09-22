using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ISetService _setService;
        public readonly string userId;

        public EditModel(ISetService setService, ICurrentUserService userService)
        {
            _setService = setService;
            userId = userService.UserId;
        }

        [BindProperty] public Set Set { get; set; }
        [BindProperty] public QuestionSet QuestionSet { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Set = await _setService.GetSetByIdAsync(id);

            if (this.Set == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }            

            try
            {
                await _setService.UpdateSetAsync(this.Set);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostDetachAsync()
        {
            try
            {
                await _setService.RemoveQuestionSetAsync(this.QuestionSet);
            }
            catch
            {
                NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}