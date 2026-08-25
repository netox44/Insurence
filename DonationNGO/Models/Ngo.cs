using System;
using System.ComponentModel.DataAnnotations;

namespace Insurence.Models
{
    public class Ngo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string? FocusArea { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Url]
        public string Website { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime PartnershipDate { get; set; } = DateTime.Now;

        // Optional User Relationship
        public string? UserId { get; set; }
    }
}
