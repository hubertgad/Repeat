using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Pages.TakeTest
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ITestService _testService;
        public string UserId { get; }

        public IndexModel(ITestService testService, ICurrentUserService currentUserService)
        {
            _testService = testService;
            UserId = currentUserService.UserId;
        }

        public List<Set> Sets { get; set; }

        public async Task OnGetAsync()
        {
            this.Sets = await _testService.GetAvailableSetsAsync();
        }
    }
}