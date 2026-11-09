namespace Infrastructure.Persistence;

using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<AdmissionCourse> AdmissionCourses => Set<AdmissionCourse>();
    public DbSet<Product> Products => Set<Product>();
}