using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    public class EditModel : CustomPageModel
    {
        public EditModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base(userManager, questionService)
        {
        }

        [BindProperty] public Question Question { get; set; }
        [BindProperty] public FileUpload FileUpload { get; set; }
        [BindProperty] public List<int> SelectedSets { get; set; }
        [BindProperty] public bool RemovePicture { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? answers)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);

            while (answers > this.Question.Answers.Count && answers < 11)
            {
                Question.Answers.Add(new Answer { QuestionID = this.Question.ID });
            }

            if (this.Question == null)
            {
                return NotFound();
            }

            await BindDataToViewAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Question question)
        {
            if (!ModelState.IsValid) return Page();

            var success = await UpdateQuestionAsync(question);
            if (success == false) return NotFound();

            return RedirectToPage($"./Details", new { question.ID });
        }

        public async Task<IActionResult> OnPostRemoveAsync(Question question, int? answerId)
        {
            if (!ModelState.IsValid) return Page();

            question.Answers[(int)answerId].IsDeleted = true;

            var success = await UpdateQuestionAsync(question);
            if (success == false) return NotFound();

            question.Answers.Remove(question.Answers[(int)answerId]);

            return RedirectToPage(new { id = question.ID, answers = question.Answers.Count });
        }

        public async Task<IActionResult> OnPostAddAsync(Question question)
        {
            if (!ModelState.IsValid) return Page();

            var success = await UpdateQuestionAsync(question);
            if (success == false) return NotFound();

            return RedirectToPage(new { id = question.ID, answers = question.Answers.Count + 1 });
        }

        private async Task BindDataToViewAsync()
        {
            IEnumerable<int> selectedSetsValues = Question.QuestionSets.Select(q => q.SetID);
            ViewData["CategoryID"] = new SelectList(await _qService.GetCategoryListAsync(this.CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new MultiSelectList(await _qService.GetSetListAsync(this.CurrentUserID), "ID", "Name", selectedSetsValues);
        }

        private async Task<bool> UpdateQuestionAsync(Question question)
        {
            try
            {
                question = UpdateQuestionSetsState(question);
                question = await FileUpload.UpdatePictureStateAsync(question);
                await _qService.UpdateQuestionAsync(question, RemovePicture);
                return true;
            }
            catch { return false; }
        }

        private Question UpdateQuestionSetsState(Question question)
        {
            question.QuestionSets = new HashSet<QuestionSet>();
            foreach (var setID in this.SelectedSets)
            {
                question.QuestionSets.Add(new QuestionSet
                {
                    QuestionID = question.ID,
                    SetID = setID
                });
            }
            return question;
        }
    }
}