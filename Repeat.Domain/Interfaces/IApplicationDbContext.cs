using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Repeat.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Repeat.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<QuestionSet> QuestionSets { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<ChoosenAnswer> ChoosenAnswers { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DatabaseFacade Database { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        public int SaveChanges();
        public void Dispose();
    }
}