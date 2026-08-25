using System.ComponentModel.DataAnnotations;

namespace Insurence.Models

{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
