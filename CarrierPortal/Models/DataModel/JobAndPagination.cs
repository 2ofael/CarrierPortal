namespace CarrierPortal.Models.DataModel
{
    public class JobAndPagination
    {
        public PaginatedList<Job> Paginations { get; set;}
        public JobFilterModel Filter { get; set;}

    }
}
