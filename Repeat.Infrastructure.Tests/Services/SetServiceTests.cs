using NUnit.Framework;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastructure.Services;
using Repeat.Infrastucture.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repeat.Infrastructure.Tests.Services
{
    [TestFixture]
    class SetServiceTests : RepeatTestBase
    {
        private ISetService _setService;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _setService = new SetService(_serviceContext, _currentUserService);
        }

        [Test]
        public async Task AddSetAsync_WhenCalled_ShouldSaveSetInDb()
        {
            var initialCount = _setUpContext.Sets.Count();
            var set = new Set { Id = 5 };

            await _setService.AddSetAsync(set);

            var currentCount = _context.Sets.Count();
            Assert.That(currentCount, Is.GreaterThan(initialCount));
        }

        [Test]
        public async Task AddSetAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var set = new Set { Id = 5, OwnerId = "SecondUserId" };

            await _setService.AddSetAsync(set);

            var savedSet = _context.Sets.First(q => q.Id == set.Id);
            Assert.That(savedSet.OwnerId, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task AddShareAsync_WhenCalled_ShouldSaveShareInDb()
        {
            await _setService.AddShareAsync(2, "Second User");

            var savedShare = _context.Shares
                .First(q => q.SetId == 2 && q.UserId == "SecondUserId");
            Assert.That(savedShare, Is.Not.Null);
        }

        [Test]
        public async Task AddShareAsync_WrongUserName_ShouldNotSaveShare()
        {
            await _setService.AddShareAsync(2, "Wrong Name");

            var savedShare = _context.Shares.FirstOrDefault(q => q.SetId == 2);
            Assert.That(savedShare, Is.Null);
        }

        [Test]
        public void AddShareAsync_CurrentUserIsNotSetOwner_ThrowNotValidOwnerException()
        {
            _setUpContext.Sets.Add(new Set { Id = 5, OwnerId = "SecondUserId" });
            _setUpContext.SaveChanges();


            Assert.That(async () => await _setService.AddShareAsync(5, "Third User"),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task AddShareAsync_TryToShareToOwner_ShouldNotSaveShare()
        {
            await _setService.AddShareAsync(2, "User");

            var savedShare = _context.Shares.FirstOrDefault(q => q.SetId == 2);
            Assert.That(savedShare, Is.Null);
        }

        [Test]
        public async Task RemoveSetAsync_WhenCalled_ShouldRemoveSetFromDb()
        {
            var set = _setUpContext.Sets.Find(1);

            await _setService.RemoveSetAsync(set);

            var removedSet = _context.Sets.FirstOrDefault(q => q.Id == 1);
            Assert.That(removedSet, Is.Null);
        }

        [Test]
        public void RemoveSetAsync_NotValidOwnerId_ThrowNotValidOwnerException()
        {
            var set = new Set { Id = 5, OwnerId = "SecondUserId" };
            _setUpContext.Sets.Add(set);
            _setUpContext.SaveChanges();

            Assert.That(async () => await _setService.RemoveSetAsync(set),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task RemoveSetAsync_WhenCalled_ShouldRemoveAssociatedTests()
        {
            var set = _setUpContext.Sets.Find(1);

            await _setService.RemoveSetAsync(set);

            var removedTests = _context.Tests.Where(q => q.SetId == 1).ToList();
            Assert.That(removedTests, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionFromSetAsync_WhenCalled_ShouldRemoveQuestion()
        {
            var questionSet = _setUpContext.QuestionSets.Find(1, 1);

            await _setService.RemoveQuestionFromSetAsync(questionSet);

            var removedQuestionSet = _context.QuestionSets
                .FirstOrDefault(q => q.QuestionId == 1 && q.SetId == 1);
            Assert.That(removedQuestionSet, Is.Null);
        }

        [Test]
        public void RemoveQuestionFromSetAsync_NotValidOwnerId_ThrowNotValidOwnerException()
        {
            var questionSet = _setUpContext.QuestionSets.Find(3, 3);

            Assert.That(async () => await _setService.RemoveQuestionFromSetAsync(questionSet),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task RemoveShareAsync_WhenCalled_ShouldRemoveShare()
        {
            var share = _setUpContext.Shares.Find(1, "SecondUserId");

            await _setService.RemoveShareAsync(share);

            var removedShare = _context.Shares
                .FirstOrDefault(q => q.SetId == 1 && q.UserId == "SecondUserId");
            Assert.That(removedShare, Is.Null);
        }

        [Test]
        public void RemoveShareAsync_NotValidOwnerId_ThrowNotValidOwnerIdException()
        {
            var share = _setUpContext.Shares.Find(3, "ThirdUserId");

            Assert.That(async () => await _setService.RemoveShareAsync(share),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task UpdateSetAsync_WhenCalled_ShouldUpdateSet()
        {
            var set = _setUpContext.Sets.Find(1);
            set.Name = "New name";

            await _setService.UpdateSetAsync(set);

            var setInDb = _context.Sets.Find(1);
            Assert.That(setInDb.Name, Is.EqualTo("New name"));
        }

        [Test]
        public void UpdateSetAsync_NotValidOwnerId_ThrowNotValidOwnerException()
        {
            var set = _setUpContext.Sets.Find(3);
            set.Name = "New name";

            Assert.That(async () => await _setService.UpdateSetAsync(set),
                Throws.Exception.TypeOf<AccessDeniedException>());
        }

        [Test]
        public async Task GetSetByIdAsync_WhenCalled_ShouldReturnSetAsync()
        {
            var result = await _setService.GetSetByIdAsync(1);

            Assert.That(result, Is.TypeOf<Set>());
        }

        [Test]
        public async Task GetSetByIdAsync_WhenCalled_ShouldContainQuestionsAsync()
        {
            var result = await _setService.GetSetByIdAsync(1);

            Assert.That(result.QuestionSets.FirstOrDefault().Question, Is.TypeOf<Question>());
        }

        [Test]
        public async Task GetSetByIdAsync_NotValidOwnerId_ShouldReturnNullAsync()
        {
            var result = await _setService.GetSetByIdAsync(3);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetSetsForCurrentUserAsync_WhenCalled_ShouldReturnListOfSetsAsync()
        {
            var result = await _setService.GetSetsForCurrentUserAsync();

            Assert.That(result, Is.TypeOf<List<Set>>());
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public async Task GetSetsForCurrentUserAsync_WhenCalled_ShouldContainUserReferenceAsync()
        {
            var result = await _setService.GetSetsForCurrentUserAsync();
            var share = result.FirstOrDefault().Shares.FirstOrDefault();

            Assert.That(share.User, Is.TypeOf<ApplicationUser>());
            Assert.That(share.User.Id, Is.TypeOf<string>());
        }
    }
}