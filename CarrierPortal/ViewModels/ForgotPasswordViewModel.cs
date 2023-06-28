using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
