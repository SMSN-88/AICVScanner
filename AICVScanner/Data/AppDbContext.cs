using AICVScanner.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AICVScanner.Data;
public class AppDbContext : IdentityDbContext<ApplicationUser>

{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    public DbSet<CvDocument> CvDocuments => Set<CvDocument>();
    public DbSet<ScanResult> ScanResults => Set<ScanResult>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Configure relationships and constraints
        builder.Entity<CvDocument>()
            .HasOne(c => c.User)
            .WithMany(u => u.CvDocuments)
            .HasForeignKey(c => c.UserId);
        builder.Entity<ScanResult>()
     .HasOne(s => s.User)
     .WithMany(u => u.ScanResults)
     .HasForeignKey(s => s.UserId)
     .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<ScanResult>()
            .HasOne(s => s.CvDocument)
            .WithMany(c => c.ScanResults)
            .HasForeignKey(s => s.CvDocumentId);

        builder.Entity<CvDocument>()
            .Property(c => c.ExtractedText)
            .HasColumnType("nvarchar(max)");

        builder.Entity<ScanResult>()
            .Property(s => s.JobDescription)
            .HasColumnType("nvarchar(max)");

        builder.Entity<ScanResult>()
           .Property(s => s.MissingSkills)
           .HasColumnType("nvarchar(max)");

        builder.Entity<ScanResult>()
            .Property(s => s.Suggestions)
            .HasColumnType("nvarchar(max)");

        builder.Entity<ScanResult>()
            .Property(s => s.KeywordAnalysis)
            .HasColumnType("nvarchar(max)");
    }
}


// Next Step in my project: RevalidatingIdentityAuthenticationStateProvider
