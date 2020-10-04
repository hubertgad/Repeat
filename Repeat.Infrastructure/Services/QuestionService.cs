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
            model.OwnerId = _currentUserId;
            await _context.Questions.AddAsync(model);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionAsync(Question model)
        {
            _context.TestQuestions.RemoveRange(_context.TestQuestions.Where(q => q.QuestionId == model.Id));
            _context.ChoosenAnswers.RemoveRange(_context.ChoosenAnswers.Where(q => q.QuestionId == model.Id));
            _context.Questions.Remove(model);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question model, bool removePicture)
        {
            model.OwnerId = _currentUserId;

            var currentQuestionSets = _context.QuestionSets.Where(q => q.QuestionId == model.Id).AsEnumerable();
            _context.QuestionSets.RemoveRange(currentQuestionSets.Except(model.QuestionSets));
            _context.QuestionSets.AddRange(model.QuestionSets.Except(currentQuestionSets));

            var answersToRemove = _context.Answers.Where(q => q.QuestionId == model.Id).AsEnumerable().Except(model.Answers);
            _context.Answers.RemoveRange(answersToRemove);

            var picture = await _context.Pictures.FirstOrDefaultAsync(q => q.Id == model.Id);
            if (removePicture || (picture != null && model.Picture != null))
            {
                _context.Pictures.Remove(picture);
            }

            _context.Questions.Update(model);

            await _context.SaveChangesAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int? id)
        {
            var question = await _context.Questions
                .Where(q => q.OwnerId == _currentUserId)
                .Include(q => q.Owner)
                .Include(q => q.Category)
                .Include(q => q.Picture)
                .Include(q => q.QuestionSets).ThenInclude(q => q.Set)
                .FirstOrDefaultAsync(q => q.Id == id);

            question.Answers = await _context.Answers
                .Where(q => q.QuestionId == id).ToListAsync();

            return question;
        }

        public async Task<List<Question>> GetQuestionListAsync(int? catId, int? setId)
        {
            var query = _context.Questions
                .Include(q => q.QuestionSets)
                .Where(q => q.OwnerId == _currentUserId);

            if (setId != null) query = query.Where(q => q.QuestionSets.Any(p => p.SetId == setId));

            if (catId != null) query = query.Where(q => q.CategoryId == catId);

            var questions = await query.ToListAsync();

            foreach (var question in questions)
            {
                question.Answers =
                    await _context.Answers
                    .Where(q => q.QuestionId == question.Id)
                    .ToListAsync();
            }

            return questions;
        }

        public Task<List<Set>> GetSetListAsync()
        {
            return _context.Sets
                .Include(q => q.Shares)
                .Where(q => q.OwnerId == _currentUserId)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Category)
                .ToListAsync();
        }

        public Task<List<Category>> GetCategoryListAsync()
        {
            return _context.Categories.Where(q => q.OwnerId == _currentUserId).ToListAsync();
        }
    }
}