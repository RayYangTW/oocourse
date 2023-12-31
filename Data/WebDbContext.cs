using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using personal_project.Models.Domain;

namespace personal_project.Data
{
  public class WebDbContext : DbContext
  {
    public WebDbContext(DbContextOptions options) : base(options)
    {

    }

    // User related
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }

    // Teacher related
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<TeacherApplication> TeacherApplications { get; set; }
    public DbSet<Certification> Certifications { get; set; }
    public DbSet<Course> Courses { get; set; }

    // Course related
    public DbSet<CourseCategory> CourseCategories { get; set; }
    public DbSet<ChatRecord> ChatRecords { get; set; }
    public DbSet<CourseRecord> CourseRecords { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CourseAccessList> CourseAccessLists { get; set; }

    // Order related
    public DbSet<Booking> Bookings { get; set; }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //   modelBuilder.Entity<User>()
    //       .HasOne(e => e.teacherApplication)
    //       .WithOne(e => e.user)
    //       .HasForeignKey<TeacherApplication>(e => e.id);
    // }
  }
}