using Microsoft.AspNetCore.Http;
using Repeat.Domain.Interfaces;
using System.Security.Claims;

namespace Repeat.DataAccess.Services
{
    class CurrentUserService : ICurrentUserService
    {
        public string UserId { get; set; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}