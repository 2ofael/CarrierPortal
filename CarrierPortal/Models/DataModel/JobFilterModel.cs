namespace CarrierPortal.Models.DataModel
{
    public class JobFilterModel
    {

        public string Title { get; set; }
       
        public string Description { get; set; }
       
        public int  MaxSalary { get; set; }

        public int MinSalary { get; set; }

        public string Location { get; set; }

        public DateTime PostedAfterDate{ get; set; }

        public int  MinApplicants { get; set; }
        public int MaxApplicants { get; set; }



    }
}
