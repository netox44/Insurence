using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Insurence.Models

{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select an image.")]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Optional: Track which user uploaded the image
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Microsoft.AspNetCore.Identity.IdentityUser User { get; set; }
    }
}
