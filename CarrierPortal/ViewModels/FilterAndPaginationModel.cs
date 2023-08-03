using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;

namespace CarrierPortal.ViewModels
{
    public class FilterAndPaginationModel
    {
      public  PaginatedList<Actor> paginatedList { get; set; }

      public MentorFilter mentorFilter { get; set; }

        

    }
}
