using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repeat.DataAccess.Data;
using Repeat.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    public class QuestionService
    {
        private readonly ApplicationDbContext _context;

        public QuestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool QuestionExists(int id) => _context.Questions.Any(e => e.ID == id);

        public bool AnswerExists(int id) => _context.Answers.Any(e => e.ID == id);

        public bool SetExists(int id) => _context.Sets.Any(e => e.ID == id);

        public bool ShareExists(Share share) => _context.Shares.Any(e => e == share);

        public async Task<List<Question>> GetQuestionListAsync(string userID, int? categoryID = null, int? setID = null)
        {
            if (categoryID == null && setID == null)
            {
                return await _context
                    .Questions
                    .Where(q => q.OwnerID == userID && q.IsDeleted == false)
                    .ToListAsync();
            }
            else if (categoryID == null)
            {
                return await _context.Questions
                    .Where(q => q.QuestionSets.Any(p => p.SetID == setID)
                        && q.OwnerID == userID
                        && q.IsDeleted == false)
                    .ToListAsync();
            }
            else if (setID == null)
            {
                return await _context.Questions
                    .Where(q => q.CategoryID == categoryID
                        && q.OwnerID == userID
                        && q.IsDeleted == false)
                    .ToListAsync();
            }
            else
            {
                return await _context.Questions
                    .Where(q => q.CategoryID == categoryID
                        && q.QuestionSets.Any(p => p.SetID == setID)
                        && q.OwnerID == userID
                        && q.IsDeleted == false)
                    .ToListAsync();
            }
        }

        public async Task<List<Category>> GetCategoryListAsync(string userID)
            => await _context
            .Categories
            .Where(q => q.OwnerID == userID && q.IsDeleted == false)
            .ToListAsync();

        public async Task<List<Set>> GetSetListAsync(string userID)
            => await _context.Sets.Include(q => q.Shares).Where(q => q.OwnerID == userID).ToListAsync();

        public async Task<List<QuestionSet>> GetQuestionSetListAsync(Question question)
        {
            return await _context.QuestionSets.Where(q => q.QuestionID == question.ID).ToListAsync();
        }

        public async Task<List<IdentityUser>> GetUserListAsync(string userID) 
            => await _context.Users.Where(q => q.Id != userID).ToListAsync();

        public async Task<Question> GetQuestionByIDAsync(int questionID, string userID)
        {
            return await _context
                .Questions
                .Where(m => m.OwnerID == userID && m.IsDeleted == false)
                .Include(o => o.Category)
                .Include(n => n.Answers)
                .Include(p => p.Picture)
                .Include(r => r.QuestionSets).ThenInclude(q => q.Set)
                .FirstOrDefaultAsync(m => m.ID == questionID);
        }

        public async Task<Category> GetCategoryByIDAsync(int categoryID, string userID)
        {
            return await _context
                .Categories
                .Where(m => m.OwnerID == userID && m.IsDeleted == false)
                .FirstOrDefaultAsync(m => m.ID == categoryID);
        }

        public async Task<Set> GetSetByIDAsync(int setID, string userID)
        {
            return await _context
                .Sets
                .Where(m => m.OwnerID == userID)
                .Include(o => o.QuestionSets).ThenInclude(q => q.Question)
                .FirstOrDefaultAsync(m => m.ID == setID);
        }

        public async Task<QuestionSet> GetQuestionSetAsync(int setID, int questionID)
        {
            return await _context
                .QuestionSets
                .FirstOrDefaultAsync(q => q.SetID == setID && q.QuestionID == questionID);
        }

        public async Task CreateQuestionAsync(Question question)
        {
            question.IsDeleted = false;
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task CreateCategoryAsync(Category category)
        {
            category.IsDeleted = false;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task CreateSetAsync(Set set)
        {
            await _context.Sets.AddAsync(set);
            foreach (var share in set.Shares)
            {
                await _context.Shares.AddAsync(share);
            }
            await _context.SaveChangesAsync();
        }

        public async Task CreateShareAsync(Share share)
        {
            await _context.Shares.AddAsync(share);
            await _context.SaveChangesAsync();
        }

        public void EditQuestion(Question question)
        {
            _context.Attach(question).State = EntityState.Modified;

            foreach (var answer in question.Answers)
            {
                _context.Attach(answer).State = EntityState.Modified;
            }

            foreach (var questionSet in question.QuestionSets)
            {
                _context.QuestionSets.Add(questionSet);
            }

            _context.SaveChanges();
        }

        public async Task EditCategoryAsync(Category category)
        {
            _context.Attach(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task EditSetAsync(Set set)
        {
            _context.Attach(set).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public void RemoveQuestionSetsRange(Question question)
        {
            _context.QuestionSets.RemoveRange(_context.QuestionSets.Where(o => o.QuestionID == question.ID));
            _context.SaveChanges();
        }

        public void AddNewAnswer(int questionID) => _context.Add(new Answer { QuestionID = questionID, AnswerText = "Type answer text..." });

        public void RemoveAnswer(int answerID)
        {
            var answer = _context.Answers.Find(answerID);
            _context.DeletedAnswers.Add(new DeletedAnswer(answer));
            _context.Remove(_context.Answers.Find(answerID));
        }

        public async Task RemovePictureAsync(Question question)
        {
            _context.Pictures.Remove(question.Picture);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSetAsync(Set set)
        {
            _context.Sets.Remove(set);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionFromSetAsync(QuestionSet questionSet)
        {
            _context.QuestionSets.Remove(questionSet);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveShareAsync(Share share)
        {
            _context.Shares.Remove(share);
            await _context.SaveChangesAsync();
        }

        public async Task MarkQuestionAsDeleted(Question question)
        {
            _context.Attach(question).State = EntityState.Modified;
            question.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
        
        public async Task MarkCategoryAsDeleted(Category category)
        {
            foreach (var question in _context.Questions.Where(q => q.CategoryID == category.ID).ToList())
            {
                await MarkQuestionAsDeleted(question);
            }

            _context.Attach(category).State = EntityState.Modified;
            category.IsDeleted = true;
            _context.SaveChanges();
        }
    }
}