using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastructure.Services;
using Repeat.Infrastucture.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Tests.Services
{
    [TestFixture]
    class QuestionServiceTests : RepeatTestBase
    {
        private IQuestionService _questionService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _questionService = new QuestionService(_serviceContext, _currentUserService);
        }

        [Test]
        public async Task AddQuestionAsync_WhenCalled_SaveQuestionInDbAsync()
        {
            var initialCount = _setUpContext.Questions.Count();
            var question = new Question();

            await _questionService.AddQuestionAsync(question);

            var currentCount = _context.Questions.Count();
            Assert.That(currentCount, Is.GreaterThan(initialCount));
        }

        [Test]
        public async Task AddQuestionAsync_NotValidOwnerId_CorrectIdAsync()
        {
            var question = new Question { OwnerId = "not-valid-id" };

            await _questionService.AddQuestionAsync(question);

            var savedQuestion = await _context.Questions
                .FirstAsync(q => q.Id == question.Id);
            Assert.That(savedQuestion.OwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_RemoveQuestionFromDbAsync()
        {
            var question = await _setUpContext.Questions.FirstAsync();
            var initialCount = _setUpContext.Questions.Count();

            await _questionService.RemoveQuestionAsync(question);

            var currentCount = _context.Questions.Count();
            Assert.That(currentCount, Is.EqualTo(initialCount - 1));
            Assert.That(async () => await _context.Questions.FirstAsync(q => q == question), 
                Throws.Exception);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_RemoveAssociatedQuestionSetsFromDbAsync()
        {
            var question = await _setUpContext.Questions.FirstAsync();

            await _questionService.RemoveQuestionAsync(question);

            var associatedQuestionSets = _context.QuestionSets
                .Where(q => q.QuestionId == question.Id)
                .ToList();
            Assert.That(associatedQuestionSets, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_RemoveAssociatedTestQuestionsFromDbAsync()
        {
            var question = await _setUpContext.Questions.FirstAsync();

            await _questionService.RemoveQuestionAsync(question);

            var associatedTestQuestions = _context.TestQuestions
                .Where(q => q.QuestionId == question.Id)
                .ToList();
            Assert.That(associatedTestQuestions, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_RemoveAssociatedChoosenAnswersFromDbAsync()
        {
            var question = await _setUpContext.Questions.FirstAsync();

            await _questionService.RemoveQuestionAsync(question);

            var associatedChoosenAnswers = _context.ChoosenAnswers
                .Where(q => q.QuestionId == question.Id)
                .ToList();
            Assert.That(associatedChoosenAnswers, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_UserIsNotOwner_ThrowAccessDeniedExceptionAsync()
        {
            var question = await _setUpContext.Questions.FindAsync(3);

            Assert.That(() => _questionService.RemoveQuestionAsync(question),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        //#TODO: UpdateQuestionAsync() method tests

        /*
        [Test]
        public async Task UpdateQuestionAsync_WhenCalled_UpdateQuestionInDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            question.QuestionText = "New question text";

            await _questionService.UpdateQuestionAsync(question, false);

            var savedQuestion = await _context.Questions.FirstAsync(q => q.Id == question.Id);
            Assert.That(savedQuestion.QuestionText, Is.EqualTo("New question text"));
        }


        [Test]
        public async Task UpdateQuestionAsync_RemovePictureIsTrue_RemovePictureFromDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            Assert.That(question.Picture, Is.Not.Null);

            await _questionService.UpdateQuestionAsync(question, true);

            var savedQustion = _context.Questions.First(q => q == question);
            Assert.That(savedQustion.Picture, Is.Null);
        }

        [Test]
        public async Task UpdateQuestionAsync_RemovedAnswer_RemoveAnswerFromDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            var answersCount = question.Answers.Count;
            question.Answers.RemoveAt(0);

            await _questionService.UpdateQuestionAsync(question, false);

            var questionInDb = _context.Questions.First(q => q.Id == question.Id);
            Assert.That(question.Answers.Count, Is.EqualTo(answersCount - 1));
        }
        */

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ReturnQuestionAsync()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question, Is.TypeOf<Question>());
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ContainCategoryAsync()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Category, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ContainPictureAsync()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Picture, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ContainSetsAsync()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.QuestionSets.First().Set, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ContainAnswersAsync()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Answers, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ContainOwnerAsync()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Owner, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ReturnTwoQuestionsAsync()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ContainQuestionSetsAsync()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.TrueForAll(q => q.QuestionSets != null));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ContainAnswersAsync()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.TrueForAll(q => q.Answers != null));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCategoryIdIsPassed_AllReturnedQuestionsShouldBelongToThisCategoryAsync()
        {
            var questions = await _questionService.GetQuestionListAsync(1, null);

            Assert.That(questions.TrueForAll(q => q.CategoryId == 1));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenSetIdIsPassed_AllReturnedQuestionsShouldBelongToThisSetAsync()
        {
            var questions = await _questionService.GetQuestionListAsync(null, 1);

            Assert.That(questions.TrueForAll(q => q.QuestionSets.Any(w => w.SetId == 1)));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ReturnTwoSetsAsync()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ContainSharesAsync()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets.TrueForAll(q => q.Shares != null));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ContainQuestionsWithCategoriesAsync()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets
                .TrueForAll(q => q.QuestionSets
                    .Select(w => w.Question.Category) != null));
        }

        [Test]
        public async Task GetCategoryListAsync_WhenCalled_ReturnTwoCategoriesAsync()
        {
            var categories = await _questionService.GetCategoryListAsync();

            Assert.That(categories.Count, Is.EqualTo(2));
        }
    }
}