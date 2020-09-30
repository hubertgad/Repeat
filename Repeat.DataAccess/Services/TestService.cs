using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.DataAccess.Services
{
    public class TestService : ITestService
    {
        private readonly IApplicationDbContext _context;
        private readonly string _currentUserId;

        public TestService(IApplicationDbContext context, ICurrentUserService user)
        {
            _context = context;
            _currentUserId = user.UserId;
        }

        public async Task AddTestAsync(Test model)
        {
            _context.Tests.Add(model);
            await _context.SaveChangesAsync();

            //EF nie chce przypisywać TestID podczas tworzenia ChoosenAnswers! :(
            foreach (var testQuestion in model.TestQuestions)
            {
                foreach (var choosenAnswer in testQuestion.ChoosenAnswers)
                {
                    choosenAnswer.TestID = model.ID;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateTestFromSetAsync(int? setId)
        {
            if (setId == null) return;

            var set = await _context.Sets
                .FirstOrDefaultAsync(q => q.ID == setId && 
                (q.OwnerID == _currentUserId || q.Shares.Any(q => q.UserID == _currentUserId))); 
            
            var test = new Test
            {
                SetID = (int)setId,
                Set = await _context.Sets.FirstOrDefaultAsync(q => q.ID == setId),
                UserID = _currentUserId,
                IsCompleted = false,
                TestQuestions = new List<TestQuestion>()
            };

            var questions = await _context.Questions
                .Where(q => q.QuestionSets.Any(w => w.SetID == (int)setId))
                .Include(q => q.Answers)
                .ToListAsync();

            foreach (var question in questions)
            {
                int i = questions.IndexOf(question);
                if (i == 0) test.CurrentQuestionID = question.ID;

                test.TestQuestions.Add(
                    new TestQuestion
                    {
                        TestID = test.ID,
                        QuestionID = question.ID,
                        Question = question,
                        ChoosenAnswers = new List<ChoosenAnswer>()
                    });

                foreach (var answer in question.Answers)
                {
                    test.TestQuestions[i].ChoosenAnswers.Add(
                        new ChoosenAnswer
                        {
                            TestID = test.ID,
                            QuestionID = question.ID,
                            AnswerID = answer.ID,
                        });
                }
            }

            await AddTestAsync(test);            
        }

        public Task UpdateTestAsync(Test model)
        {
            _context.Tests.Update(model);
            return _context.SaveChangesAsync();
        }

        public async Task UpdateChoosenAnswersAsync(IList<ChoosenAnswer> choosenAnswers)
        {
            var currentAnswers = await _context.ChoosenAnswers
                .Where(q => q.TestID == choosenAnswers.FirstOrDefault().TestID
                    && q.QuestionID == choosenAnswers.FirstOrDefault().QuestionID)
                .ToListAsync();

            for (int i = 0; i < currentAnswers.Count(); i++)
            {
                currentAnswers[i].GivenAnswer = choosenAnswers[i].GivenAnswer;
            }

            await _context.SaveChangesAsync();
        }

        public async Task MoveToPreviousQuestion(int? setId)
        {
            var test = _context.Tests
                .FirstOrDefault(q => q.SetID == setId && q.IsCompleted == false && q.UserID == _currentUserId);

            var currentQuestionIndex = test.TestQuestions.IndexOf(
                test.TestQuestions.FirstOrDefault(q => q.QuestionID == test.CurrentQuestionID));
            test.CurrentQuestionID = test.TestQuestions[currentQuestionIndex - 1].QuestionID;
            
            await _context.SaveChangesAsync();
        }

        public async Task MoveToNextQuestion(int? setId)
        {
            var test = _context.Tests
                .FirstOrDefault(q => q.SetID == setId && q.IsCompleted == false && q.UserID == _currentUserId);

            var currentQuestionIndex = test.TestQuestions.IndexOf(
                test.TestQuestions.FirstOrDefault(q => q.QuestionID == test.CurrentQuestionID));
            test.CurrentQuestionID = test.TestQuestions[currentQuestionIndex + 1].QuestionID;
            
            await _context.SaveChangesAsync();
        }

        public async Task FinishTest(int? setId)
        {
            var test = _context.Tests
                .FirstOrDefault(q => q.SetID == setId && q.IsCompleted == false && q.UserID == _currentUserId);

            test.IsCompleted = true;
            
            await _context.SaveChangesAsync();
        }

        public Task<Test> GetOpenTestBySetIdAsync(int? setId)
        {
            return _context.Tests
                .Where(q => q.UserID == _currentUserId && q.SetID == setId && q.IsCompleted == false)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Answers)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Picture)
                .FirstOrDefaultAsync();
        }

        public async Task<Test> GetClosedTestBySetIdAsync(int? setId)
        {
            var tests = await _context.Tests
                .Where(q => q.UserID == _currentUserId && q.SetID == setId && q.IsCompleted == true)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Answers)
                .Include(q => q.TestQuestions)
                    .ThenInclude(p => p.ChoosenAnswers)
                .Include(q => q.TestQuestions)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Picture)
                .Include(q => q.Set)
                .ToListAsync();

            return tests.LastOrDefault();
        }

        public Task<List<ChoosenAnswer>> GetChoosenAnswersAsync(int? testId, int? questionId)
        {
            return _context.ChoosenAnswers
                .Where(q => q.TestID == testId && q.QuestionID == questionId)
                .ToListAsync();
        }

        public Task<List<Set>> GetAvailableSetsAsync()
        {
            return _context.Sets
                .Where(q => q.OwnerID == _currentUserId || q.Shares.Any(p => p.UserID == _currentUserId))
                .Where(q => q.QuestionSets.Any())
                .Include(q => q.Shares)
                .Include(q => q.QuestionSets)
                    .ThenInclude(q => q.Question)
                    .ThenInclude(q => q.Category)
                .Include(q => q.Owner)
                .ToListAsync();
        }
    }
}