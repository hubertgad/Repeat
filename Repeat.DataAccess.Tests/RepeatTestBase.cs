using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repeat.DataAccess.Data;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using System;
using System.Collections.Generic;

namespace Repeat.DataAccess.Services
{
    [SetUpFixture]
    public class RepeatTestBase
    {
        protected IApplicationDbContext _setUpContext;
        protected IApplicationDbContext _context;
        protected ICurrentUserService _userService;

        [SetUp]
        public virtual void SetUp()
        {
            _userService = new CurrentUserService { UserId = Guid.NewGuid().ToString() };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _setUpContext = new ApplicationDbContext(options);
            _context = new ApplicationDbContext(options);

            _setUpContext.Database.EnsureCreated();

            SeedTestDb();
        }

        [TearDown]
        public void TearDown()
        {
            _setUpContext.Database.EnsureDeleted();

            _setUpContext.Dispose();
            _context.Dispose();
        }

        private void SeedTestDb()
        {
            var categories = new List<Category>
            {
                new Category { ID = 1, Name = "Category 1", OwnerID = _userService.UserId },
                new Category { ID = 2, Name = "Category 2", OwnerID = _userService.UserId }
            };

            var sets = new List<Set>
            {
                new Set { ID = 1, Name = "Set 1", OwnerID = _userService.UserId },
                new Set { ID = 2, Name = "Set 2", OwnerID = _userService.UserId }
            };

            var questions = new List<Question>
            {
                new Question
                {
                    ID = 1,
                    QuestionText = "Question 1",
                    OwnerID = _userService.UserId,
                    Picture = new Picture { Data = new byte[] { 255, 255, 255 } },
                    CategoryID = 1,
                    Answers = new List<Answer>
                    {
                        new Answer { QuestionID = 1, AnswerText = "Answer 1-1" },
                        new Answer { QuestionID = 1, AnswerText = "Answer 1-2" }
                    },
                    QuestionSets = new HashSet<QuestionSet>
                    {
                        new QuestionSet { QuestionID = 1, SetID = 1 },
                        new QuestionSet { QuestionID = 1, SetID = 2 }
                    }
                },
                new Question
                {
                    ID = 2,
                    QuestionText = "Question 2",
                    OwnerID = _userService.UserId,
                    Picture = new Picture { Data = new byte[] { 255, 255, 255 } },
                    CategoryID = 2,
                    Answers = new List<Answer>
                    {
                        new Answer { QuestionID = 2, AnswerText = "Answer 2-1" },
                        new Answer { QuestionID = 2, AnswerText = "Answer 2-2" }
                    },
                    QuestionSets = new HashSet<QuestionSet>
                    {
                        new QuestionSet { QuestionID = 2, SetID = 1 },
                        new QuestionSet { QuestionID = 2, SetID = 2 }
                    }
                },
            };

            _setUpContext.Categories.AddRange(categories);

            _setUpContext.Sets.AddRange(sets);

            _setUpContext.Questions.AddRange(questions);

            _setUpContext.SaveChanges();
        }
    }
}