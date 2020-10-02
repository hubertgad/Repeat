using NUnit.Framework;
using Repeat.Infrastructure.Services;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
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

            _setService = new SetService(_context, _currentUserService);
        }

        [Test]
        public async Task AddSetAsync_WhenCalled_ShouldSaveSetInDb()
        {
            var set = new Set { ID = 5 };

            await _setService.AddSetAsync(set);

            var savedSet = _context.Sets.First(q => q.ID == set.ID);
            Assert.That(savedSet, Is.EqualTo(set));
        }

        [Test]
        public async Task AddSetAsync_NotValidOwnerId_ShouldCorrectId()
        {
            var set = new Set { ID = 5, OwnerID = "SecondUserId" };

            await _setService.AddSetAsync(set);

            var savedSet = _context.Sets.First(q => q.ID == set.ID);
            Assert.That(savedSet.OwnerID, Is.EqualTo(_currentUserService.UserId));
        }

        [Test]
        public async Task AddShareAsync_WhenCalled_ShouldSaveShareInDb()
        {
            await _setService.AddShareAsync(2, "Second User");

            var savedShare = _context.Shares
                .First(q => q.SetID == 2 && q.UserID == "SecondUserId");
            Assert.That(savedShare, Is.Not.Null);
        }

        [Test]
        public async Task AddShareAsync_WrongUserName_ShouldNotSaveShare()
        {
            await _setService.AddShareAsync(2, "Wrong Name");

            var savedShare = _context.Shares.FirstOrDefault(q => q.SetID == 2);
            Assert.That(savedShare, Is.Null);
        }

        [Test]
        public async Task AddShareAsync_CurrentUserIsNotSetOwner_ShouldNotSaveShare()
        {
            _setUpContext.Sets.Add(new Set { ID = 5, OwnerID = "SecondUserId" });
            _setUpContext.SaveChanges();

            await _setService.AddShareAsync(5, "Third User");

            var savedShare = _context.Shares.FirstOrDefault(q => q.SetID == 5);
            Assert.That(savedShare, Is.Null);
        }

        [Test]
        public async Task AddShareAsync_TryToShareToOwner_ShouldNotSaveShare()
        {
            await _setService.AddShareAsync(2, "User");

            var savedShare = _context.Shares.FirstOrDefault(q => q.SetID == 2);
            Assert.That(savedShare, Is.Null);
        }

        [Test]
        public async Task RemoveSetAsync_WhenCalled_ShouldRemoveSetFromDb()
        {
            var set = _setUpContext.Sets.Find(1);

            await _setService.RemoveSetAsync(set);

            var removedSet = _context.Sets.FirstOrDefault(q => q.ID == 1);
            Assert.That(removedSet, Is.Null);
        }

        [Test]
        public async Task RemoveSetAsync_WhenNotValidOwnerId_ShouldNotRemoveSet()
        {
            var set = new Set { ID = 5, OwnerID = "SecondUserId" };
            _setUpContext.Sets.Add(set);
            _setUpContext.SaveChanges();
            
            await _setService.RemoveSetAsync(set);

            var setInDb = _context.Sets.FirstOrDefault(q => q.ID == 5);
            Assert.That(setInDb, Is.Not.Null);
        }

        [Test]
        public async Task RemoveSetAsync_WhenCalled_ShouldRemoveAssociatedTests()
        {
            var set = _setUpContext.Sets.Find(1);

            await _setService.RemoveSetAsync(set);

            var removedTests = _context.Tests.Where(q => q.SetID == 1).ToList();
            Assert.That(removedTests, Is.Empty);
        }

        [Test]
        public async Task RemoveQuestionFromSetAsync_WhenCalled_ShouldRemoveQuestion()
        {
            var questionSet = _setUpContext.QuestionSets.Find(1, 1);

            await _setService.RemoveQuestionFromSetAsync(questionSet);

            var removedQuestionSet = _context.QuestionSets
                .FirstOrDefault(q => q.QuestionID == 1 && q.SetID == 1);
            Assert.That(removedQuestionSet, Is.Null);
        }

        [Test]
        public async Task RemoveQuestionFromSetAsync_WhenNotValidOwnerId_ShouldNotRemoveQuestion()
        {
            var questionSet = _setUpContext.QuestionSets.Find(3, 3);

            await _setService.RemoveQuestionFromSetAsync(questionSet);

            var questionSetInDb = _context.QuestionSets
                .FirstOrDefault(q => q.QuestionID == 3 && q.SetID == 3);
            Assert.That(questionSetInDb, Is.Not.Null);
        }

        // ...
    }
}