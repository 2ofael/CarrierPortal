using CarrierPortal.Models.DataModel;
using Microsoft.AspNetCore.Identity;

namespace CarrierPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool isProfileUpdated { get; set; } = false;
        public string City { get; set; }

        public Actor Mentor { get; set; }

    }
}
