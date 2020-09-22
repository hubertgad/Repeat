using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ISetService _setService;
        public DeleteModel(ISetService setService)
        {
            _setService = setService;
        }

        [BindProperty] public Set Set { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Set = await _setService.GetSetByIdAsync(id);

            if (Set != null)
            {
                await _setService.RemoveSetAsync(this.Set);
            }

            return RedirectToPage("./Index");
        }
    }
}