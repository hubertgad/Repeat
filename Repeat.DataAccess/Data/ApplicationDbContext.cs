﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repeat.Domain.Interfaces;
using Repeat.Domain.Models;

namespace Repeat.DataAccess.Data
{
    public partial class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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

            builder.Entity<Share>()
                .HasKey(su => new { su.SetID, su.UserID });
            builder.Entity<Share>()
                .HasOne(su => su.Set)
                .WithMany(su => su.Shares)
                .HasForeignKey(su => su.SetID);

            builder.Entity<TestQuestion>()
                .HasKey(tq => new { tq.TestID, tq.QuestionID });
            builder.Entity<TestQuestion>()
                .HasOne(tq => tq.Test)
                .WithMany(tq => tq.TestQuestions)
                .HasForeignKey(tq => tq.TestID);

            builder.Entity<TestQuestion>()
                .HasMany(q => q.ChoosenAnswers)
                .WithOne(q => q.TestQuestion)
                .HasForeignKey(q => new { q.TestID, q.QuestionID })
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}