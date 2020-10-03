using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ISetService _setService;
        public IndexModel(ISetService setService)
        {
            _setService = setService;
        }

        public List<Set> Set { get; set; }

        public async Task OnGetAsync()
        {
            this.Set = await _setService.GetSetsForCurrentUserAsync();
        }
    }
}