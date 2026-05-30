using Microsoft.EntityFrameworkCore;
using Prn232.Lab1.Repositories.Domain;

namespace Prn232.Lab1.Repositories
{
    public class Prn232Lab1DbContext : DbContext
    {
        public Prn232Lab1DbContext()
        {
        }

        public Prn232Lab1DbContext(DbContextOptions<Prn232Lab1DbContext> options)
            : base(options)
        {
        }

        public DbSet<Semester> Semesters => Set<Semester>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Semester>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Semester)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
                entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(20).IsRequired();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
