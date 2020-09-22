using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public EditModel(IQuestionService questionService)
        {
            _questionService = questionService;
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

            this.Question = await _questionService.GetQuestionByIdAsync(id);

            while (answers > this.Question.Answers.Count && answers < 11)
            {
                this.Question.Answers.Add(new Answer { QuestionID = this.Question.ID });
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

        public async Task<IActionResult> OnPostRemoveAsync(Question question, int? answerIndex)
        {
            if (!ModelState.IsValid) return Page();

            question.Answers.RemoveAt((int)answerIndex);

            var success = await UpdateQuestionAsync(question);
            if (success == false) return NotFound();
                       
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
            ViewData["CategoryID"] = new SelectList(await _questionService.GetCategoryListAsync(), "ID", "Name");
            ViewData["SetID"] = new MultiSelectList(await _questionService.GetSetListAsync(), "ID", "Name", selectedSetsValues);
        }

        private async Task<bool> UpdateQuestionAsync(Question question)
        {
            try
            {
                UpdateQuestionSets(ref question);
                question = await FileUpload.UpdatePictureAsync(question);
                await _questionService.UpdateQuestionAsync(question, this.RemovePicture);
                return true;
            }
            catch { return false; }
        }

        private void UpdateQuestionSets(ref Question question)
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
        }
    }
}