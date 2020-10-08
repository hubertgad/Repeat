using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;
using Repeat.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Repeat.Infrastructure.Services
{
    public class RepeatTestBase
    {
        protected ApplicationDbContext _setUpContext;
        protected ApplicationDbContext _context;
        protected ApplicationDbContext _serviceContext;
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
            _serviceContext = new ApplicationDbContext(options);

            _setUpContext.Database.EnsureCreated();

            SeedTestDb();
        }

        [TearDown]
        public void TearDown()
        {
            _setUpContext.Database.EnsureDeleted();

            _setUpContext.Dispose();
            _context.Dispose();
            _serviceContext.Dispose();
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
                        new Answer { Id = 1, QuestionId = 1, AnswerText = "Answer 1-1" },
                        new Answer { Id = 2, QuestionId = 1, AnswerText = "Answer 1-2" }
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
                        new Answer { Id = 3, QuestionId = 2, AnswerText = "Answer 2-1" },
                        new Answer { Id = 4, QuestionId = 2, AnswerText = "Answer 2-2" }
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
                        new Answer { Id = 5, QuestionId = 3, AnswerText = "Answer 3-1" },
                        new Answer { Id = 6, QuestionId = 3, AnswerText = "Answer 3-2" }
                    },
                    QuestionSets = new HashSet<QuestionSet>
                    {
                        new QuestionSet { QuestionId = 3, SetId = 3 }
                    }
                }
            };

            var tests = new List<Test>()
            {
                new Test
                {
                    Id = 1,
                    SetId = 1,
                    CurrentQuestionId = 2,
                    IsCompleted = true,
                    UserId = _currentUserService.UserId,
                    TestQuestions = new List<TestQuestion>
                    {
                        new TestQuestion
                        {
                            QuestionId = 1,
                            TestId = 1,
                            ChoosenAnswers = new List<ChoosenAnswer>
                            {
                                new ChoosenAnswer
                                {
                                    Id = 1,
                                    AnswerId = 1,
                                    QuestionId = 1,
                                    TestId = 1,
                                    GivenAnswer = false
                                },
                                new ChoosenAnswer
                                {
                                    Id = 2,
                                    AnswerId = 2,
                                    QuestionId = 1,
                                    TestId = 1,
                                    GivenAnswer = false
                                }
                            }
                        },
                        new TestQuestion
                        {
                            QuestionId = 2,
                            TestId = 1,
                            ChoosenAnswers = new List<ChoosenAnswer>()
                        },
                        new TestQuestion
                        {
                            QuestionId = 3,
                            TestId = 1,
                            ChoosenAnswers = new List<ChoosenAnswer>()
                        }
                    }
                },
                new Test
                {
                    Id = 2,
                    SetId = 2,
                    CurrentQuestionId = 0,
                    IsCompleted = false,
                    UserId = "SecondUserId",
                    TestQuestions = new List<TestQuestion>()
                },
                new Test
                {
                    Id = 3,
                    SetId = 1,
                    CurrentQuestionId = 2,
                    IsCompleted = false,
                    UserId = _currentUserService.UserId,
                    TestQuestions = new List<TestQuestion>
                    {
                        new TestQuestion
                        {
                            QuestionId = 1,
                            TestId = 3,
                            ChoosenAnswers = new List<ChoosenAnswer>
                            {
                                new ChoosenAnswer
                                {
                                    Id = 3,
                                    AnswerId = 1,
                                    QuestionId = 1,
                                    TestId = 3,
                                    GivenAnswer = false
                                },
                                new ChoosenAnswer
                                {
                                    Id = 4,
                                    AnswerId = 2,
                                    QuestionId = 1,
                                    TestId = 3,
                                    GivenAnswer = false
                                }
                            }
                        },
                        new TestQuestion
                        {
                            QuestionId = 2,
                            TestId = 3,
                            ChoosenAnswers = new List<ChoosenAnswer>()
                        },
                        new TestQuestion
                        {
                            QuestionId = 3,
                            TestId = 3,
                            ChoosenAnswers = new List<ChoosenAnswer>()
                        }
                    }
                }
            };

            _setUpContext.Users.AddRange(users);

            _setUpContext.Categories.AddRange(categories);

            _setUpContext.Sets.AddRange(sets);

            _setUpContext.Questions.AddRange(questions);

            _setUpContext.Tests.AddRange(tests);

            _setUpContext.SaveChanges();
        }
    }
}