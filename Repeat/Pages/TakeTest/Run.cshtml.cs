using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [BindProperty] public List<ChoosenAnswer> ChoosenAnswers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            this.Test = await _testService.GetOpenTestBySetIdAsync((int)id);

            if (this.Test == null)
            {
                await _testService.CreateTestFromSetAsync((int)id);
                this.Test = await _testService.GetOpenTestBySetIdAsync((int)id);

                if (this.Test == null)
                {
                    return NotFound();
                }
            }
            this.CurrentQuestionIndex = this.Test.TestQuestions.IndexOf(
                this.Test.TestQuestions
                .FirstOrDefault(q => q.QuestionId == this.Test.CurrentQuestionId));

            this.ChoosenAnswers = await _testService.GetChoosenAnswersAsync(this.Test.Id, this.Test.CurrentQuestionId);

            return Page();
        }

        public async Task<IActionResult> OnPostFinishAsync(int? id)
        {
            if (id == null) return NotFound();

            return await OnPostCommonAsync((int)id, () => _testService.FinishTest((int)id));
        }

        public async Task<IActionResult> OnPostPreviousAsync(int? id)
        {
            if (id == null) return NotFound();

            return await OnPostCommonAsync((int)id, () => _testService.MoveToPreviousQuestion((int)id));
        }


        public async Task<IActionResult> OnPostNextAsync(int? id)
        {
            if (id == null) return NotFound();

            return await OnPostCommonAsync((int)id, () => _testService.MoveToNextQuestion((int)id));
        }

        [NonHandler]
        private async Task<IActionResult> OnPostCommonAsync(int id, Func<Task> task)
        {
            if (!ModelState.IsValid) return Page();

            this.Test = await _testService.GetOpenTestBySetIdAsync(id);

            this.ChoosenAnswers.ToList()
                    .ForEach(q =>
                    {
                        q.TestId = this.Test.Id;
                        q.QuestionId = this.Test.CurrentQuestionId;
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