using CarrierPortal.Models.DataModel;

namespace CarrierPortal.Repository
{
    public interface IActorRepository
    {
        Task AddActor(Actor actor);
        Task DeleteActor(Actor actor);
        Task<Actor> GetActorById(string id);
        Task<List<Actor>> GetAllActors();
        Task UpdateActor(Actor actor);
    }
}