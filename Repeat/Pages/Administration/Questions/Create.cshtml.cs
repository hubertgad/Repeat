using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repeat.Data;
using Repeat.Models;

namespace Repeat.Pages.Administration.Questions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public Question Question { get; set; }
        [BindProperty]
        public FileUpload FileUpload { get; set; }
        [BindProperty]
        public int[] SelectedSets { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            BindDataToView();
            this.Question = new Question { Answers = new List<Answer>() };
            for (int i = 0; i < 4; i++)
            {
                this.Question.Answers.Add(new Answer());
            }
            this.Question.OwnerID = await GetUserIDAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            BindDataToView();
            if (!ModelState.IsValid)
            {
                return Page();
            }
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

            _context.Questions.Add(this.Question);
            await _context.SaveChangesAsync();

            foreach (var item in SelectedSets)
            {
                _context.QuestionSets.Add(new QuestionSet
                {
                    QuestionID = this.Question.ID,
                    SetID = item
                });
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        private void BindDataToView()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name");
            ViewData["SetID"] = new SelectList(_context.Sets, "ID", "Name");
        }
        private async Task<string> GetUserIDAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return user.Id;
        }
    }
}