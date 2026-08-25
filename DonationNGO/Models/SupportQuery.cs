using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurence.Models
{
    public class SupportQuery
    {
        [Key]
        public int Id { get; set; }

        // FK to Identity User
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Microsoft.AspNetCore.Identity.IdentityUser User { get; set; }

        [Required]
        [StringLength(250)]
        public string Subject { get; set; }

        [StringLength(50)]
        public string Priority { get; set; } = "Normal";

        [Required]
        [StringLength(2000)]
        public string Message { get; set; }

        [StringLength(255)]
        public string AttachmentPath { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Status { get; set; } = "Open";
    }
}
