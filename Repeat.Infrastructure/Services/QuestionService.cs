using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _currentUserId;

        public QuestionService(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserId = currentUserService.UserId;
        }

        public async Task AddQuestionAsync(Question model)
        {
            model.OwnerID = _currentUserId;
            await _context.Questions.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionAsync(Question model)
        {
            _context.TestQuestions.RemoveRange(_context.TestQuestions.Where(q => q.QuestionID == model.ID));
            _context.ChoosenAnswers.RemoveRange(_context.ChoosenAnswers.Where(q => q.QuestionID == model.ID));
            _context.Questions.Remove(model);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question model, bool removePicture)
        {
            model.OwnerID = _currentUserId;

            var currentQuestionSets = _context.QuestionSets.Where(q => q.QuestionID == model.ID).ToList();
            _context.QuestionSets.RemoveRange(currentQuestionSets.Except(model.QuestionSets));
            _context.QuestionSets.AddRange(model.QuestionSets.Except(currentQuestionSets));

            var answersToRemove = _context.Answers.Where(q => q.QuestionID == model.ID).ToList().Except(model.Answers);
            _context.Answers.RemoveRange(answersToRemove);

            if (removePicture == true)
            {
                _context.Pictures.Remove(model.Picture);
            }

            _context.Questions.Update(model);

            await _context.SaveChangesAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int? id)
        {
            var question = await _context.Questions
                .Where(q => q.OwnerID == _currentUserId)
                .Include(q => q.Owner)
                .Include(q => q.Category)
                .Include(q => q.Picture)
                .Include(q => q.QuestionSets).ThenInclude(q => q.Set)
                .FirstOrDefaultAsync(q => q.ID == id);

            question.Answers = await _context.Answers
                .Where(q => q.QuestionID == id).ToListAsync();

            return question;
        }

        public async Task<List<Question>> GetQuestionListAsync(int? catId, int? setId)
        {
            var query = _context.Questions
                .Include(q => q.QuestionSets)
                .Where(q => q.OwnerID == _currentUserId);
            
            if (setId != null) query = query.Where(q => q.QuestionSets.Any(p => p.SetID == setId));
            
            if (catId != null) query = query.Where(q => q.CategoryID == catId);

            var questions = await query.ToListAsync();

            foreach (var question in questions)
            {
                question.Answers =
                    await _context.Answers
                    .Where(q => q.QuestionID == question.ID)
                    .ToListAsync();
            }

            return questions;
        }

        public Task<List<Set>> GetSetListAsync()
        {
            return _context.Sets
                .Include(q => q.Shares)
                .Where(q => q.OwnerID == _currentUserId)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Category)
                .ToListAsync();
        }

        public Task<List<Category>> GetCategoryListAsync()
        {
            return _context.Categories.Where(q => q.OwnerID == _currentUserId).ToListAsync();
        }
    }
}