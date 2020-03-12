using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repeat.DataAccess.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repeat.Pages
{
    [Authorize]
    public class CustomPageModel : PageModel
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly QuestionService _qService;

        public string CurrentUserID
        {
            get { return GetUserIDAsync().Result; }
        }

        public CustomPageModel()
        {
        }

        public CustomPageModel(UserManager<IdentityUser> userManager, QuestionService questionService)
        {
            _userManager = userManager;
            _qService = questionService;
        }

        protected async Task<string> GetUserIDAsync() => (await _userManager.GetUserAsync(User)).Id;

        public List<string> SplitTextByPattern(string input)
        {
            List<string> lines = new List<string>();
            lines.Add("");

            foreach (Match m in Regex.Matches(input, @"<code>(.+?)<\/code>|(.| )?", RegexOptions.Singleline))
            {
                if (m.Value.Contains(@"<code>") && (m.Value.Contains(@"</code>")))
                {
                    if (lines[lines.Count - 1].ToString() == "")
                    {
                        lines[lines.Count - 1] = m.Value;
                    }
                    else
                    {
                        lines.Add(m.Value);
                    }
                    lines.Add("");
                }
                else
                {
                    lines[lines.Count - 1] += m.Value;
                }
            }
            return lines;
        }
    }
}