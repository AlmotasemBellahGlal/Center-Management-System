using Center_Management.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Context
{
    public class CenterDBContext : IdentityDbContext<AppUser>
    {
        public CenterDBContext(DbContextOptions<CenterDBContext> options) : base(options) { }

        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Matrial> Matrials { get; set; }
        public DbSet<Attendence> Attendences { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<GroupSchedule> GroupSchedules { get; set; }

        // Exam system
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentExamAttempt> StudentExamAttempts { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentGroup>()
                .HasKey(sg => new
                {
                    sg.StudentId,
                    sg.GroupId
                });

            modelBuilder.Entity<StudentGroup>()
                .HasOne(sg => sg.Student)
                .WithMany(s => s.StudentGroups)
                .HasForeignKey(sg => sg.StudentId);

            modelBuilder.Entity<StudentGroup>()
                .HasOne(sg => sg.Group)
                .WithMany(g => g.StudentGroups)
                .HasForeignKey(sg => sg.GroupId);

            // Payment: Unique (StudentId, GroupId, Month, Year)
            modelBuilder.Entity<Payment>()
                .HasIndex(p => new { p.StudentId, p.GroupId, p.Month, p.Year })
                .IsUnique();

            // Payment FK → Group
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Payments)
                .HasForeignKey(p => p.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Attendence: Unique (StudentId, GroupId, Date)
            modelBuilder.Entity<Attendence>()
                .HasIndex(a => new { a.StudentId, a.GroupId, a.Date })
                .IsUnique();

            // Attendence FK → Group
            modelBuilder.Entity<Attendence>()
                .HasOne(a => a.Group)
                .WithMany(g => g.Attendences)
                .HasForeignKey(a => a.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // One attempt per student per exam
            modelBuilder.Entity<StudentExamAttempt>()
                .HasIndex(a => new { a.StudentId, a.ExamId })
                .IsUnique();

            // StudentAnswer → StudentExamAttempt (Restrict to avoid cascade cycles)
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Attempt)
                .WithMany(a => a.Answers)
                .HasForeignKey(sa => sa.AttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            // AppUser → Student (optional link)
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Student)
                .WithMany()
                .HasForeignKey(u => u.StudentId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
