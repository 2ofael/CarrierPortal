using CarrierPortal.Models.DataModel;

namespace CarrierPortal.Repository
{
    public interface IJobRepository
    {
        Task CreateApplicantAsync(Applicant applicant);
        Task CreateJobAsync(Job job);
        Task CreateResumeAsync(Resume resume);
        Task DeleteApplicantAsync(Applicant applicant);
        Task DeleteJobAsync(Job job);
        Task DeleteResumeAsync(Resume resume);
        Task<List<Applicant>> GetAllApplicantsAsync();
        Task<List<Job>> GetAllJobsAsync();
        Task<List<Resume>> GetAllResumesAsync();
        Task<Applicant> GetApplicantByIdAsync(int applicantId);
        Task<Job> GetJobByIdAsync(int jobId);
        Task<Resume> GetResumeByIdAsync(int resumeId);
        Task UpdateApplicantAsync(Applicant applicant);
        Task UpdateJobAsync(Job job);
        Task UpdateResumeAsync(Resume resume);
    }
}