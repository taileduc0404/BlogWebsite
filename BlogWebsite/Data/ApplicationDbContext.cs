using BlogWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogWebsite.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }

        public DbSet<ApplicationUser>? applicationUsers { get; set; }
        public DbSet<Post>? posts { get; set; }
        public DbSet<Page>? pages { get; set; }
        public DbSet<Setting>? settings { get; set; }
        public DbSet<SmtpSettings>? smtpSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SmtpSettings>().Ignore(s => s.Id);
        }

    }
}
