using CarrierPortal.Models.DataModel;
using CarrierPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Repository
{
    public class ActorRepository : IActorRepository
    {
        private readonly AppDbContext _dbContext;

        public ActorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Actor>> GetAllActors()
        {
            return await _dbContext.Actors.ToListAsync();
        }

        public async Task<Actor> GetActorById(string id)
        {
            return await _dbContext.Actors.FindAsync(id);
        }

        public async Task AddActor(Actor actor)
        {
            await _dbContext.Actors.AddAsync(actor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateActor(Actor actor)
        {
            _dbContext.Actors.Update(actor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActor(Actor actor)
        {
            _dbContext.Actors.Remove(actor);
            await _dbContext.SaveChangesAsync();
        }


    }
}
