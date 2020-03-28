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
using Repeat.Domain.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class EditModel : CustomPageModel
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

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);
            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();

            try
            {
                await _qService.UpdateAsync(this.Question);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_qService.ElementExists(this.Question))
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

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);
            Question.Answers.FirstOrDefault(q => q.ID == aid).IsDeleted = true;

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();

            try
            {
                await _qService.UpdateAsync(this.Question);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostAddAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            this.Question = await _qService.GetQuestionByIDAsync((int)id, this.CurrentUserID);

            var answer = new Answer { QuestionID = this.Question.ID, AnswerText = "Type answer text..." };
            await _qService.AddAsync(answer);

            await UpdatePictureStateAsync();
            UpdateQuestionSetsState();
            
            try
            {
                await _qService.UpdateAsync(this.Question);
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
            ViewData["CategoryID"] = new SelectList(await _qService.GetCategoryListAsync(this.CurrentUserID), "ID", "Name");
            ViewData["SetID"] = new MultiSelectList(await _qService.GetSetListAsync(this.CurrentUserID), "ID", "Name", selectedSetsValues);
        }

        private void UpdateQuestionSetsState()
        {
            this.Question.QuestionSets = new List<QuestionSet>();
            foreach (var setID in this.SelectedSets)
            {
                Question.QuestionSets.Add(new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = setID
                });
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
                this.Question.Picture = null;
            }
        }
    }
}