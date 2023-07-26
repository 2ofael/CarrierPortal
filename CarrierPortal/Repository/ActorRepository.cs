using CarrierPortal.Models.DataModel;
using CarrierPortal.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            return await _dbContext.Actors.Include(m=>m.Loved).ToListAsync();
        }

        public async Task<Actor> GetActorById(string id)
        {
            return await _dbContext.Actors
        .Include(m => m.Loved)
        .FirstOrDefaultAsync(a => a.ActorId == id);
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

        public async Task<List<Actor>> GetPostsAsync(string searchTerm, int page, int pageSize)
        {
            var query = _dbContext.Actors.Include(a=>a.User).Include(a=>a.Loved).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Skills.Contains(searchTerm) ||
                b.About.Contains(searchTerm)||
                b.ActorName.Contains(searchTerm)||
                b.CurrentProfession.Contains(searchTerm)||
                b.age.ToString().Contains(searchTerm)||
                b.Address.Contains(searchTerm)||
                b.AcademicQualification.Contains(searchTerm)
                

                );
            }

            var skip = (page - 1) * pageSize;

            if (!query.Any())
            {
                return new List<Actor>();
            }

            return await query.OrderByDescending(b => b.age)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPostsCountAsync(string searchTerm)
        {
            var query = _dbContext.Actors.Include(a => a.User).Include(a=>a.Loved).AsQueryable();
          




            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Skills.Contains(searchTerm) ||
             b.About.Contains(searchTerm) ||
             b.ActorName.Contains(searchTerm) ||
             b.CurrentProfession.Contains(searchTerm) ||
             b.age.ToString().Contains(searchTerm) ||
             b.Address.Contains(searchTerm) ||
             b.AcademicQualification.Contains(searchTerm));

            }
            return await query.CountAsync();
        }

    }
}
