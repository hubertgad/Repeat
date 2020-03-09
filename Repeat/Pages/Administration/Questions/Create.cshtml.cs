using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.DataAccess.Services;
using Repeat.Models;

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

            this.Question.Picture = await UploadPictureAsync();
            this.Question.QuestionSets = CreateListOfQuestionSets();

            try
            {
                await _qService.CreateQuestionAsync(this.Question);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        private List<QuestionSet> CreateListOfQuestionSets()
        {
            var questionSets = new List<QuestionSet>();
            foreach (var setID in SelectedSets)
            {
                questionSets = new List<QuestionSet>
                {
                new QuestionSet { QuestionID = this.Question.ID, SetID = setID }
                };
            }
            return questionSets;
        }

        private async Task<Picture> UploadPictureAsync()
        {
            if (FileUpload.FormFile != null && FileUpload.FormFile.Length > 0)
            {
                var picture = new Picture();
                using var memoryStream = new MemoryStream();
                await FileUpload.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < 2097152)
                {
                    picture.Data = memoryStream.ToArray();
                    return picture;
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }
            return null;
        }

        public IActionResult OnPostMore() => RedirectToPage(new { answers = ++AnswersCount });

        public IActionResult OnPostLess() => RedirectToPage(new { answers = --AnswersCount });
    }
}