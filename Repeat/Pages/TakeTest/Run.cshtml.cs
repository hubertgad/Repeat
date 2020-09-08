using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Services;
using Repeat.Domain.Models;
using Repeat.Pages;

namespace Repeat
{
    [Authorize]
    public class RunModel : CustomPageModel
    {
        public RunModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Test Test { get; set; }
        [BindProperty] public QuestionResponse QuestionResponse { get; set; }
        public Question CurrentQuestion { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var requestedSet = await _qService.GetSetByIDAsync((int)id, this.CurrentUserID, true);

            if (requestedSet == null)
            {
                return NotFound();
            }

            var startedTest = await _qService.GetTestByIDAsync(this.CurrentUserID, id, null);
            if (startedTest != null)
            {
                this.Test = startedTest;
            }
            else
            {
                var set = await _qService.GetSetByIDAsync((int)id, this.CurrentUserID, true);
                var questions = await _qService.GetQuestionListAsync(this.CurrentUserID, null, (int)id, true);
                this.Test = new Test(set, this.CurrentUserID, questions);

                try
                {
                    await _qService.AddAsync(this.Test);
                }
                catch
                {
                    return NotFound();
                }
            }

            this.CurrentQuestion = GetCurrentQuestion();
            this.QuestionResponse = this.Test.QuestionResponses[this.Test.CurrentQuestionIndex];
            
            return Page();
        }

        private Question GetCurrentQuestion()
        {
            return this.Test.TestQuestions
                    .Where(q => q.Index == Test.CurrentQuestionIndex)
                    .Select(q => q.Question)
                    .FirstOrDefault();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id, string options)
        {
            if (!ModelState.IsValid) return Page();

#pragma warning disable CS8509
            object _ = options switch
            {
                "Next" => this.Test.CurrentQuestionIndex++,
                "Previous" => this.Test.CurrentQuestionIndex--,
                "Finish" => this.Test.IsCompleted = true,
            };
#pragma warning restore CS8509

            try
            {
                await _qService.UpdateAsync(this.Test);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            if (this.Test.IsCompleted)  return RedirectToPage("./Details", new { id }); 
            else                        return RedirectToPage(new { id });
        }
    }
}