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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Унікальний індекс на ім’я файлу
            modelBuilder.Entity<FileUpload>()
                .HasIndex(f => f.FileName)
                .IsUnique();

            // Композитний ключ для CourseUser (багато-до-багатьох)
            modelBuilder.Entity<CourseUser>()
                .HasKey(cu => new { cu.CourseId, cu.UserId });

            modelBuilder.Entity<CourseUser>()
                .HasOne(cu => cu.Course)
                .WithMany(c => c.Users)
                .HasForeignKey(cu => cu.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.Courses)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SavedTest → User
            modelBuilder.Entity<SavedTest>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SavedDictionary → User
            modelBuilder.Entity<SavedDictionary>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
