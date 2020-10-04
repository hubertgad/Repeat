using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Repeat.Infrastructure.Services
{
    [SetUpFixture]
    public class RepeatTestBase
    {
        protected ApplicationDbContext _setUpContext;
        protected ApplicationDbContext _context;
        protected ICurrentUserService _currentUserService;

        [SetUp]
        public virtual void SetUp()
        {
            _currentUserService = new CurrentUserService { UserId = Guid.NewGuid().ToString() };

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
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = _currentUserService.UserId, UserName = "User" },
                new ApplicationUser { Id = "SecondUserId", UserName = "Second User" },
                new ApplicationUser { Id = "ThirdUserId", UserName = "Third User" }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", OwnerId = _currentUserService.UserId },
                new Category { Id = 2, Name = "Category 2", OwnerId = _currentUserService.UserId }
            };

            var sets = new List<Set>
            {
                new Set
                {
                    Id = 1,
                    Name = "Set 1",
                    OwnerId = _currentUserService.UserId,
                    Shares = new HashSet<Share>
                    {
                        new Share { SetId = 1, UserId = "SecondUserId" }
                    }
                },
                new Set
                {
                    Id = 2,
                    Name = "Set 2",
                    OwnerId = _currentUserService.UserId,
                    Shares = new HashSet<Share>()
                },
                new Set
                {
                    Id = 3,
                    Name = "Set 3",
                    OwnerId = "SecondUserId",
                    Shares = new HashSet<Share>
                    {
                        new Share { SetId = 3, UserId = "ThirdUserId" }
                    }
                }
            };

            var test = new Test
            {
                Id = 1,
                SetId = 1,
                CurrentQuestionId = 0,
                IsCompleted = false,
                UserId = _currentUserService.UserId,
                TestQuestions = new List<TestQuestion>()
            };

            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    QuestionText = "Question 1",
                    OwnerId = _currentUserService.UserId,
                    Picture = new Picture { Data = new byte[] { 255, 255, 255 } },
                    CategoryId = 1,
                    Answers = new List<Answer>
                    {
                        new Answer { QuestionId = 1, AnswerText = "Answer 1-1" },
                        new Answer { QuestionId = 1, AnswerText = "Answer 1-2" }
                    },
                    QuestionSets = new HashSet<QuestionSet>
                    {
                        new QuestionSet { QuestionId = 1, SetId = 1 },
                        new QuestionSet { QuestionId = 1, SetId = 2 }
                    }
                },
                new Question
                {
                    Id = 2,
                    QuestionText = "Question 2",
                    OwnerId = _currentUserService.UserId,
                    Picture = new Picture { Data = new byte[] { 255, 255, 255 } },
                    CategoryId = 2,
                    Answers = new List<Answer>
                    {
                        new Answer { QuestionId = 2, AnswerText = "Answer 2-1" },
                        new Answer { QuestionId = 2, AnswerText = "Answer 2-2" }
                    },
                    QuestionSets = new HashSet<QuestionSet>
                    {
                        new QuestionSet { QuestionId = 2, SetId = 2 }
                    }
                },
                new Question
                {
                    Id = 3,
                    QuestionText = "Question 3",
                    OwnerId = "SecondUserId",
                    Picture = new Picture { Data = new byte[] { 255, 255, 255 } },
                    CategoryId = 2,
                    Answers = new List<Answer>
                    {
                        new Answer { QuestionId = 3, AnswerText = "Answer 3-1" },
                        new Answer { QuestionId = 3, AnswerText = "Answer 3-2" }
                    },
                    QuestionSets = new HashSet<QuestionSet>
                    {
                        new QuestionSet { QuestionId = 3, SetId = 3 }
                    }
                }
            };

            _setUpContext.Users.AddRange(users);

            _setUpContext.Categories.AddRange(categories);

            _setUpContext.Sets.AddRange(sets);

            _setUpContext.Tests.Add(test);

            _setUpContext.Questions.AddRange(questions);

            _setUpContext.SaveChanges();
        }
    }
}