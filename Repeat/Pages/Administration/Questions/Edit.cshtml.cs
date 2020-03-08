using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Services;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class EditModel : CustomPageModelV2
    {
        public EditModel(UserManager<IdentityUser> userManager, QuestionService questionService)
            : base (userManager, questionService)
        {
        }

        [BindProperty] public Question Question { get; set; }
        [BindProperty] public FileUpload FileUpload { get; set; }
        [BindProperty] public List<int> SelectedSets { get; set; }
        [BindProperty] public bool RemovePicture { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);
            
            if (this.Question == null)
            {
                return NotFound();
            }
            
            await BindDataToViewAsync();

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();

            try
            {
                _qService.EditQuestion(this.Question);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_qService.QuestionExists(this.Question.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage($"./Details", new { id });
        }

        public async Task<IActionResult> OnPostRemoveAsync(int? id, int aid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();

            _qService.RemoveAnswer(aid);

            try
            {
                _qService.EditQuestion(this.Question);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_qService.AnswerExists((int)aid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostAddAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();

            _qService.AddNewAnswer(this.Question.ID);

            try
            {
                _qService.EditQuestion(this.Question);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            
            return RedirectToPage(new { id });
        }

        private async Task BindDataToViewAsync()
        {
            IEnumerable<int> selectedSetsValues =  Question.QuestionSets.Select(q => q.SetID);
            ViewData["CategoryID"] = new SelectList(await _qService.GetCategoriesAsync(this.CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new MultiSelectList(await _qService.GetSetsAsync(this.CurrentUserID), "ID", "Name", selectedSetsValues);
        }

        private void UpdateQuestionSetsState()
        {
            _qService.RemoveQuestionSetsRange(this.Question);
            Question.QuestionSets = _qService.GetQuestionSets(this.Question);
            foreach (var item in this.SelectedSets)
            {
                var questionSet = new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = item
                };
                if (!Question.QuestionSets.Contains(questionSet))
                    Question.QuestionSets.Add(questionSet);
            }
        }

        private async Task UpdatePictureStateAsync()
        {
            if (FileUpload.FormFile != null && FileUpload.FormFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await FileUpload.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < 2097152)
                {
                    if (this.Question.Picture != null)
                    {
                        await _qService.RemovePictureAsync(this.Question);
                    }
                    this.Question.Picture = new Picture();
                    this.Question.Picture.Data = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }
            else if (this.RemovePicture == true)
            {
                await _qService.RemovePictureAsync(this.Question);
            }
        }
    }
}