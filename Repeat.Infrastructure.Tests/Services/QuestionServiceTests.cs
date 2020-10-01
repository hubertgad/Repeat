using NUnit.Framework;
using Repeat.Infrastructure.Services;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
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

            _questionService = new QuestionService(_context, _currentUserService);
        }

        [Test]
        public async Task AddQuestionAsync_WhenCalled_ShouldSaveQuestionInDb()
        {
            var question = new Question();

            await _questionService.AddQuestionAsync(question);

            var savedQuestion = _context.Questions.First(q => q == question);
            Assert.That(savedQuestion, Is.EqualTo(question));
        }

        [Test]
        public async Task AddQuestionAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var question = new Question { OwnerID = "not-valid-id" };

            await _questionService.AddQuestionAsync(question);

            var savedOwnerId = _context.Questions.First(q => q.ID == question.ID).OwnerID;
            Assert.That(savedOwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveQuestionFromDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            var initialCount = _setUpContext.Questions.Count();

            await _questionService.RemoveQuestionAsync(question);

            var currentCount = _context.Questions.Count();
            Assert.That(currentCount, Is.EqualTo(initialCount - 1));
            Assert.That(() => _context.Questions.First(q => q == question), Throws.Exception);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveAssociatedQuestionSetsFromDb()
        {
            var question = _setUpContext.Questions.First();

            await _questionService.RemoveQuestionAsync(question);

            var associatedQuestionSets = _context.QuestionSets
                .Where(q => q.QuestionID == question.ID)
                .ToList();
            Assert.That(associatedQuestionSets, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveAssociatedTestQuestionsFromDb()
        {
            var question = _setUpContext.Questions.First();

            await _questionService.RemoveQuestionAsync(question);

            var associatedTestQuestions = _context.TestQuestions
                .Where(q => q.QuestionID == question.ID)
                .ToList();
            Assert.That(associatedTestQuestions, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionAsync_WhenCalled_ShouldRemoveAssociatedChoosenAnswersFromDb()
        {
            var question = _setUpContext.Questions.First();

            await _questionService.RemoveQuestionAsync(question);

            var associatedChoosenAnswers = _context.ChoosenAnswers
                .Where(q => q.QuestionID == question.ID)
                .ToList();
            Assert.That(associatedChoosenAnswers, Is.Empty);
        }

        //#TODO: UpdateQuestionAsync() method tests

        /*
        [Test]
        public async Task UpdateQuestionAsync_WhenCalled_ShouldUpdateQuestionInDb()
        {
            var question = _setUpContext.Questions.FirstOrDefault();
            question.QuestionText = "New name";

            await _questionService.UpdateQuestionAsync(question, false);

            var savedName = _context.Questions.First(q => q.ID == question.ID).QuestionText;
            Assert.That(savedName, Is.EqualTo("New name"));
        }

       
       [Test]
       public async Task UpdateQuestionAsync_RemovePictureIsTrue_ShouldRemovePictureFromDb()
       {
           var question = _setUpContext.Questions.FirstOrDefault();
           Assert.That(question.Picture, Is.Not.Null);

           await _questionService.UpdateQuestionAsync(question, true);

           var savedQustion = _context.Questions.First(q => q == question);
           Assert.That(savedQustion.Picture, Is.Null);
       }

       [Test]
       public async Task UpdateQuestionAsync_RemovedAnswer_ShouldRemoveAnswerFromDb()
       {
           var question = _setUpContext.Questions.FirstOrDefault();
           var answersCount = question.Answers.Count;
           question.Answers.RemoveAt(0);

           await _questionService.UpdateQuestionAsync(question, false);

           var questionInDb = _context.Questions.First(q => q.ID == question.ID);
           Assert.That(question.Answers.Count, Is.EqualTo(answersCount - 1));
       }
       */

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldReturnQuestion()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question, Is.TypeOf<Question>());
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldContainCategory()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Category, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldContainPicture()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Picture, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldContainSets()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.QuestionSets.First().Set, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldContainAnswers()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Answers, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionByIdAsync_WhenCalled_ShouldContainOwner()
        {
            var question = await _questionService.GetQuestionByIdAsync(1);

            Assert.That(question.Owner, Is.Not.Null);
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ShouldReturnTwoQuestions()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ShouldContainQuestionSets()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.TrueForAll(q => q.QuestionSets != null));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCalled_ShouldContainAnswers()
        {
            var questions = await _questionService.GetQuestionListAsync(null, null);

            Assert.That(questions.TrueForAll(q => q.Answers != null));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenCategoryIdIsPassed_AllReturnedQuestionsShouldBelongToThisCategory()
        {
            var questions = await _questionService.GetQuestionListAsync(1, null);

            Assert.That(questions.TrueForAll(q => q.CategoryID == 1));
        }

        [Test]
        public async Task GetQuestionListAsync_WhenSetIdIsPassed_AllReturnedQuestionsShouldBelongToThisSet()
        {
            var questions = await _questionService.GetQuestionListAsync(null, 1);

            Assert.That(questions.TrueForAll(q => q.QuestionSets.Any(w => w.SetID == 1)));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ShouldReturnTwoSets()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ShouldContainShares()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets.TrueForAll(q => q.Shares != null));
        }

        [Test]
        public async Task GetSetListAsync_WhenCalled_ShouldContainQuestionsWithCategories()
        {
            var sets = await _questionService.GetSetListAsync();

            Assert.That(sets
                .TrueForAll(q => q.QuestionSets
                    .Select(w => w.Question.Category) != null));
        }

        [Test]
        public async Task GetCategoryListAsync_WhenCalled_ShouldReturnTwoCategories()
        {
            var categories = await _questionService.GetCategoryListAsync();

            Assert.That(categories.Count, Is.EqualTo(2));
        }
    }
}