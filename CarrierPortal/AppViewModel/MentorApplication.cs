using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.AppViewModel
{
    public class MentorApplication
    {

        [Required]
        [Display(Name ="Mentor Name")]
        public string ActorName { get; set; }

        [Required]

        public string Email { get; set; }
        [Required]

        public string Phone { get; set; }
        [Required]

        public string Skills { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public int age { get; set; }
        [Required]

        [Display(Name = "Academic Qualification")]
        public string AcademicQualification { get; set; }
        [Required]


        [Display(Name = "Current Qualification ")]
        public string CurrentProfession { get; set; }
        [Required]

        public string Address { get; set; }
        [Required]

        public string About { get; set; }
          

        [Required]
        public IFormFile cv { get; set; }

        [Required]
        public IFormFile  ProfilePhoto { get; set; }

        [Required]
        public IFormFile Certificates { get; set; }



    }
}
