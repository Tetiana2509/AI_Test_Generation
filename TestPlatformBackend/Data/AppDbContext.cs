using Microsoft.EntityFrameworkCore;
using TestPlatformBackend.Models;

namespace TestPlatformBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FileUpload> Files { get; set; }
        public DbSet<SavedTest> Tests { get; set; }
        public DbSet<SavedDictionary> Dictionaries { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CourseUser> CourseUsers { get; set; }
        public DbSet<FullTest> FullTests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<AnswerSubmission> AnswerSubmissions { get; set; } // ⬅️ Нове!

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileUpload>()
                .HasIndex(f => f.FileName)
                .IsUnique();

            modelBuilder.Entity<CourseUser>()
                .HasKey(cu => cu.Id);

            modelBuilder.Entity<CourseUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CourseUsers)
                .HasForeignKey(cu => cu.UserId);

            modelBuilder.Entity<CourseUser>()
                .HasOne(cu => cu.Course)
                .WithMany(c => c.CourseUsers)
                .HasForeignKey(cu => cu.CourseId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FullTest>()
                .HasMany(f => f.Questions)
                .WithOne(q => q.FullTest)
                .HasForeignKey(q => q.FullTestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.AnswerOptions)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FullTest>()
                .HasMany(f => f.TestResults)
                .WithOne(r => r.FullTest)
                .HasForeignKey(r => r.FullTestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.TestResults)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔵 НОВЕ: При видаленні результату тесту — видаляються його відповіді
            modelBuilder.Entity<TestResult>()
                .HasMany(r => r.AnswerSubmissions)
                .WithOne(a => a.TestResult)
                .HasForeignKey(a => a.TestResultId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
