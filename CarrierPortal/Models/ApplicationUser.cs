using Microsoft.AspNetCore.Identity;

namespace CarrierPortal.Models
{
    public class ApplicationUser:IdentityUser
    {

        public string City { get; set; }
    }
}
