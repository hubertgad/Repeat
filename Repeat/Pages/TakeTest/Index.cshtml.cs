using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITestService _testService;

        public IndexModel(ITestService testService)
        {
            _testService = testService;
        }

        public List<Set> Sets { get; set; }

        public async Task OnGetAsync()
        {
            this.Sets = await _testService.GetAvailableSetsAsync();
        }
    }
}