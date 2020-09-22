using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RunModel : PageModel
    {
        private readonly ITestService _testService;

        public RunModel(ITestService testService)
        {
            _testService = testService;
        }
        public Test Test { get; set; }
        public int CurrentQuestionIndex { get; set; }
        [BindProperty] public IList<ChoosenAnswer> ChoosenAnswers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            this.Test = await _testService.GetOpenTestBySetIdAsync(id);

            if (this.Test == null)
            {
                await _testService.CreateTestFromSetAsync(id);
                this.Test = await _testService.GetOpenTestBySetIdAsync(id);

                if (this.Test == null)
                {
                    return NotFound();
                }
            }
            this.CurrentQuestionIndex = this.Test.TestQuestions.IndexOf(
                this.Test.TestQuestions
                .FirstOrDefault(q => q.QuestionID == this.Test.CurrentQuestionID));

            this.ChoosenAnswers = await _testService.GetChoosenAnswersAsync(this.Test.ID, this.Test.CurrentQuestionID);

            return Page();
        }

        public async Task<IActionResult> OnPostFinishAsync(int? id)
        {
            return await OnPostCommonAsync(id, () => _testService.FinishTest(id));
        }

        public async Task<IActionResult> OnPostPreviousAsync(int? id)
        {
            return await OnPostCommonAsync(id, () => _testService.MoveToPreviousQuestion(id));
        }


        public async Task<IActionResult> OnPostNextAsync(int? id)
        {
            return await OnPostCommonAsync(id, () => _testService.MoveToNextQuestion(id));
        }

        [NonHandler]
        public async Task<IActionResult> OnPostCommonAsync(int? id, Func<Task> task)
        {
            if (!ModelState.IsValid) return Page();

            this.Test = await _testService.GetOpenTestBySetIdAsync(id);

            this.ChoosenAnswers.ToList()
                    .ForEach(q => {
                        q.TestID = this.Test.ID;
                        q.QuestionID = this.Test.CurrentQuestionID;
                    });

            await task();

            try
            {
                await _testService.UpdateChoosenAnswersAsync(this.ChoosenAnswers);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            if (this.Test.IsCompleted)
            {
                return RedirectToPage("./Details", new { id });
            }
            else
            {
                return RedirectToPage(new { id });
            }
        }
    }
}