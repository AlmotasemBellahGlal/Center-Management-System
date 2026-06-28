using Center_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Center_Management.Context
{
    public class CenterDBContext : DbContext
    {
        public CenterDBContext(DbContextOptions<CenterDBContext> options) : base(options) { }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Matrial> Matrials { get; set; }
        public DbSet<Attendence> Attendences { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<GroupSchedule> GroupSchedules { get; set; }

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
        }
    }
}
