using Insurence.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Insurence.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // ============================
        // DbSets (All Models Included)
        // ============================
        public DbSet<Ngo> Ngos { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<prmodel> Programs { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<SupportQuery> SupportQueries { get; set; }

        // ============================
        // Model Configuration
        // ============================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------------------------------
            // PROGRAM MODEL (prmodel)
            // -------------------------------------------------
            modelBuilder.Entity<prmodel>()
                .Property(p => p.AmountRaised)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<prmodel>()
                .Property(p => p.FundingGoal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<prmodel>()
                .Property(p => p.ProgramName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<prmodel>()
                .Property(p => p.Category)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<prmodel>()
                .Property(p => p.Description)
                .HasMaxLength(1000)
                .IsRequired();

            // -------------------------------------------------
            // DONATION MODEL
            // -------------------------------------------------
            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Donation>()
                .Property(d => d.DonorName)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Donation>()
                .Property(d => d.Program)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Donation>()
                .Property(d => d.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Donation>()
                .Property(d => d.Status)
                .HasMaxLength(20)
                .IsRequired();

            // Optional User Relationship
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------------------------------
            // GALLERY MODEL
            // -------------------------------------------------
            modelBuilder.Entity<Gallery>()
                .Property(g => g.ImageUrl)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Gallery>()
                .Property(g => g.Category)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Gallery>()
                .Property(g => g.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Gallery>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------------------------------
            // NGO MODEL
            // -------------------------------------------------
            modelBuilder.Entity<Ngo>()
                .Property(n => n.Name)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<Ngo>()
                .Property(n => n.FocusArea)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Ngo>()
                .Property(n => n.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Ngo>()
                .Property(n => n.Status)
                .HasMaxLength(20)
                .IsRequired();

            // -------------------------------------------------
            // SUPPORT QUERY MODEL
            // -------------------------------------------------
            modelBuilder.Entity<SupportQuery>()
                .Property(s => s.Subject)
                .HasMaxLength(250)
                .IsRequired();

            modelBuilder.Entity<SupportQuery>()
                .Property(s => s.Priority)
                .HasMaxLength(50);

            modelBuilder.Entity<SupportQuery>()
                .Property(s => s.Message)
                .HasMaxLength(2000)
                .IsRequired();

            modelBuilder.Entity<SupportQuery>()
                .Property(s => s.AttachmentPath)
                .HasMaxLength(255);

            modelBuilder.Entity<SupportQuery>()
                .Property(s => s.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");

            modelBuilder.Entity<SupportQuery>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------------------
            // Indexes For Performance
            // ----------------------
            modelBuilder.Entity<Donation>()
                .HasIndex(d => d.UserId);

            modelBuilder.Entity<SupportQuery>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<Gallery>()
                .HasIndex(g => g.Category);

            modelBuilder.Entity<prmodel>()
                .HasIndex(p => p.Category);
        }
    }
}
