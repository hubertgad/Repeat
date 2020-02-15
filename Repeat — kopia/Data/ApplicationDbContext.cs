using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repeat.Models;

namespace Repeat.Data
{
    public partial class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<QuestionSet> QuestionSets { get; set; }
        public DbSet<SetUser> SetUsers { get; set; }
        public DbSet<Picture> Pictures { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<QuestionSet>()
                .HasKey(qs => new { qs.QuestionID, qs.SetID });
            builder.Entity<QuestionSet>()
                .HasOne(qs => qs.Question)
                .WithMany(q => q.QuestionSets)
                .HasForeignKey(qs => qs.QuestionID);
            builder.Entity<QuestionSet>()
                .HasOne(qs => qs.Set)
                .WithMany(s => s.QuestionSets)
                .HasForeignKey(qs => qs.SetID);
            builder.Entity<SetUser>()
                .HasKey(su => new { su.SetID, su.UserID });
            builder.Entity<SetUser>()
                .HasOne(su => su.Set)
                .WithMany(su => su.SetUsers)
                .HasForeignKey(su => su.SetID);
        }
    }
}