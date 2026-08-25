using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Insurence.Models

{
    public class prmodel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Program name is required")]
        [StringLength(150)]
        public string ProgramName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100)]
        public string Category { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal FundingGoal { get; set; }

        [Range(1, 120)]
        public int DurationMonths { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AmountRaised { get; set; } = 0;

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Active";

        // Optional: Track who created this program
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Microsoft.AspNetCore.Identity.IdentityUser User { get; set; }

        // ------------------------- NOT MAPPED FIELDS -------------------------
        [NotMapped]
        public int DonorsCount { get; set; } = 0;

        [NotMapped]
        public decimal ProgressPercentage =>
            FundingGoal > 0
                ? Math.Min((AmountRaised / FundingGoal) * 100, 100)
                : 0;
    }

}
