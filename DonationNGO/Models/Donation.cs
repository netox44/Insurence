using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurence.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string DonorName { get; set; }

        [Required]
        [StringLength(150)]
        public string Program { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Completed, Pending, Failed

        // -------------------------------
        // Link to Identity User
        // -------------------------------
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Microsoft.AspNetCore.Identity.IdentityUser User { get; set; }
    }
}
