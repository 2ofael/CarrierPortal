namespace CarrierPortal.Models.DataModel
{
    public class ActorLoved
    {
        public int Id { get; set; }
        public string UserNameIdentifier { get; set; }

        public string ActorId { get; set; }
        public Actor Actor { get; set; }

    }
}
