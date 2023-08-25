using CarrierPortal.Models.DataModel;
using CarrierPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Repository
{
    public class JobRepository : IJobRepository
    {

        private readonly AppDbContext _dbContext;
        public JobRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Job operations

        public async Task<List<Job>> GetAllJobsAsync()
        {
            return await _dbContext.Jobs.ToListAsync();
        }

        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            return await _dbContext.Jobs.FindAsync(jobId);
        }

        public async Task<int> CreateJobAsync(Job job)
        {
            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();
            return job.Id;
        }

        public async Task UpdateJobAsync(Job job)
        {
            _dbContext.Entry(job).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteJobAsync(Job job)
        {
            _dbContext.Jobs.Remove(job);
            await _dbContext.SaveChangesAsync();
        }

        // Applicant operations

        public async Task<List<Applicant>> GetAllApplicantsAsync()
        {
            return await _dbContext.Applicants.Include(a=>a.Resume).ToListAsync();
        }

        public async Task<Applicant> GetApplicantByIdAsync(int applicantId)
        {
            return await _dbContext.Applicants.FindAsync(applicantId);
        }

        public async Task CreateApplicantAsync(Applicant applicant)
        {
            _dbContext.Applicants.Add(applicant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateApplicantAsync(Applicant applicant)
        {
            _dbContext.Entry(applicant).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteApplicantAsync(Applicant applicant)
        {
            _dbContext.Applicants.Remove(applicant);
            await _dbContext.SaveChangesAsync();
        }

        // Resume operations

        public async Task<List<Resume>> GetAllResumesAsync()
        {
            return await _dbContext.Resumes.ToListAsync();
        }

        public async Task<Resume> GetResumeByIdAsync(int resumeId)
        {
            return await _dbContext.Resumes.FindAsync(resumeId);
        }

        public async Task CreateResumeAsync(Resume resume)
        {
            _dbContext.Resumes.Add(resume);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateResumeAsync(Resume resume)
        {
            _dbContext.Entry(resume).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteResumeAsync(Resume resume)
        {
            _dbContext.Resumes.Remove(resume);
            await _dbContext.SaveChangesAsync();
        }






        public async Task<List<Job>> GetPostsAsync(string searchTerm, int page, int pageSize)
        {
            var query = _dbContext.Jobs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Description.Contains(searchTerm)|| b.Salary.ToString().Contains(searchTerm) || b.Location.Contains(searchTerm));
            }

            var skip = (page - 1) * pageSize;

            if (!query.Any())
            {
                return new List<Job>();
            }

            return await query.OrderByDescending(b => b.PostedDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPostsCountAsync(string searchTerm)
        {
            var query = _dbContext.Jobs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Description.Contains(searchTerm) || b.Salary.ToString().Contains(searchTerm) || b.Location.Contains(searchTerm));
            }

            return await query.CountAsync();
        }




    }
}
