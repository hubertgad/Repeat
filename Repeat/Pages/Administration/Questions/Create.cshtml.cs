using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IQuestionService _questionService;
        public CreateModel(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [BindProperty] public Question Question { get; set; }
        [BindProperty] public FileUpload FileUpload { get; set; }
        [BindProperty] public List<int> SelectedSets { get; set; }
        [BindProperty] public int AnswersCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? answers)
        {
            ValidateAnswersCount(answers);
            await BindDataToViewAsync();
            CreateNewQuestion();

            return Page();
        }

        private void ValidateAnswersCount(int? answers)
        {
            if (answers == null)
            {
                this.AnswersCount = 4;
            }
            else if (answers >= 10)
            {
                this.AnswersCount = 10;
            }
            else if (answers <= 2)
            {
                this.AnswersCount = 2;
            }
            else
            {
                this.AnswersCount = (int)answers;
            }
        }

        private async Task BindDataToViewAsync()
        {
            ViewData["CategoryId"] = new SelectList(await _questionService.GetCategoryListAsync(), "Id", "Name");
            ViewData["SetId"] = new SelectList(await _questionService.GetSetListAsync(), "Id", "Name");
        }

        private void CreateNewQuestion()
        {
            this.Question = new Question
            {
                Answers = new List<Answer>()
            };
            for (int i = 0; i < this.AnswersCount; i++)
            {
                this.Question.Answers.Add(new Answer());
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            this.Question = await FileUpload.UpdatePictureAsync(this.Question);
            this.Question.QuestionSets = CreateListOfQuestionSets();

            try
            {
                await _questionService.AddQuestionAsync(this.Question);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        private HashSet<QuestionSet> CreateListOfQuestionSets()
        {
            var questionSets = new HashSet<QuestionSet>();
            foreach (var setId in SelectedSets)
            {
                questionSets = new HashSet<QuestionSet>
                {
                    new QuestionSet {
                        QuestionId = this.Question.Id,
                        SetId = setId
                    }
                };
            }
            return questionSets;
        }

        public IActionResult OnPostMore() => RedirectToPage(new { answers = ++AnswersCount });

        public IActionResult OnPostLess() => RedirectToPage(new { answers = --AnswersCount });
    }
}