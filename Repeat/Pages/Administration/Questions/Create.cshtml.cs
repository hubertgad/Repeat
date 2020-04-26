using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.DataAccess.Services;
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class CreateModel : CustomPageModel
    {
        public CreateModel(UserManager<IdentityUser> userManager, QuestionService questionService) 
            : base(userManager, questionService) 
        {
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
            ViewData["CategoryID"] = new SelectList(await _qService.GetCategoryListAsync(this.CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new SelectList(await _qService.GetSetListAsync(this.CurrentUserID), "ID", "Name");
        }

        private void CreateNewQuestion()
        {
            this.Question = new Question
            {
                OwnerID = this.CurrentUserID,
                Answers = new List<Answer>()
            };
            for (int i = 0; i < this.AnswersCount; i++)
            {
                this.Question.Answers.Add(new Answer());
            }
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            this.Question.Picture = UploadPicture();
            this.Question.QuestionSets = CreateListOfQuestionSets();

            try
            {
                await _qService.AddAsync(this.Question);
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
            foreach (var setID in SelectedSets)
            {
                questionSets = new HashSet<QuestionSet>
                {
                new QuestionSet { QuestionID = this.Question.ID, SetID = setID }
                };
            }
            return questionSets;
        }

        private Picture UploadPicture()
        {
            if (FileUpload.ToByteArray(this.FileUpload) != null)
            {
                return new Picture { Data = FileUpload.ToByteArray(this.FileUpload) };
            }
            else
            {
                ModelState.AddModelError("File", "The file is too large.");
                return null;
            }
        }

        public IActionResult OnPostMore() => RedirectToPage(new { answers = ++AnswersCount });

        public IActionResult OnPostLess() => RedirectToPage(new { answers = --AnswersCount });
    }
}