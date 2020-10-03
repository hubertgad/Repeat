using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Threading.Tasks;

namespace Repeat
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ISetService _setService;
        public CreateModel(ISetService setService)
        {
            _setService = setService;
        }

        [BindProperty] public Set Set { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _setService.AddSetAsync(this.Set);

            return RedirectToPage("./Index");
        }
    }
}