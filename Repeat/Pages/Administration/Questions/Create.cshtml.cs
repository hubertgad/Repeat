using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class CreateModel : CustomPageModel
    {

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager) 
            : base(context, userManager) 
        {
        }

        [BindProperty] public Question Question { get; set; }
        [BindProperty] public FileUpload FileUpload { get; set; }
        [BindProperty] public int[] SelectedSets { get; set; }
        [BindProperty] public int AnswersCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? answers)
        {
            this.CurrentUserID = await GetUserIDAsync();

            ValidateAnswersCount(answers);
            BindDataToView();
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

        private void BindDataToView()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories.Where(q => q.OwnerID == CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new SelectList(_context.Sets.Where(q => q.OwnerID == CurrentUserID), "ID", "Name");
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

            await UploadPictureAsync();

            await UpdateQuestionStateAsync();
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            await UpdateSetsStateAsync();
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        private async Task UpdateSetsStateAsync()
        {
            foreach (var item in SelectedSets)
            {
                await _context.QuestionSets.AddAsync(new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = item
                });
            }
        }

        private async Task UpdateQuestionStateAsync() => await _context.Questions.AddAsync(this.Question);

        private async Task UploadPictureAsync()
        {
            if (FileUpload.FormFile != null && FileUpload.FormFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await FileUpload.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < 2097152)
                {
                    this.Question.Picture = new Picture();
                    this.Question.Picture.Data = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }
        }

        public IActionResult OnPostMore() => RedirectToPage(new { answers = ++AnswersCount });

        public IActionResult OnPostLess() => RedirectToPage(new { answers = --AnswersCount });
    }
}