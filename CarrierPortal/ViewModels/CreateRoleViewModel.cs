using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }

    }
}
