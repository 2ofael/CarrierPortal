namespace CarrierPortal.Models.DataModel
{
    public class Love
    {
        public int Id { get; set; }
        public string UserNameIdentifier { get; set; }

        public int BlogId { get; set; }
        public BlogPost BlogPost { get; set; }
       
    }
}
