using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;

namespace CarrierPortal.Models.DataModel
{
    public class Actor
    {
      
        public string ActorId { get; set; }

        public ApplicationUser User { get; set; }
                      
        public string UserId { get; set; }
        public string ActorName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Skills { get; set; }
        public string Gender { get; set; }
        public int age { get; set; }

        public string AcademicQualification { get; set; }

        public string CurrentProfession { get; set; }

        public string Address { get; set; }

        public string About { get; set; }

        public bool isMentor { get; set; }

        public bool isMantee { get; set; }

        public bool isSubscribed { get; set; }

        public string cv { get; set; }

        public string ProfilePhoto {get;set;}

        public string Certificates { get; set; }

        public int Votes { get; set; }
        public List<ActorLoved> Loved { get; set; }





    }
}
