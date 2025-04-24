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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileUpload>()
                .HasIndex(f => f.FileName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
