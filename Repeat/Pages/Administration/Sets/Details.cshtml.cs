using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Threading.Tasks;

namespace Repeat
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ISetService _setService;
        public readonly string userId;
        public DetailsModel(ISetService setService, ICurrentUserService userService)
        {
            _setService = setService;
            userId = userService.UserId;
        }

        public Set Set { get; set; }

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
    }
}