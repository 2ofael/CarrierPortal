using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobController(IJobRepository jobRepository, UserManager<ApplicationUser> userManager)
        {
            _jobRepository = jobRepository;
            _userManager = userManager;
        }

        // Job Actions

        //public async Task<IActionResult> Index(string search, int page = 1)
        //{
        //    const int pageSize = 10;

        //    // Retrieve the paginated job list based on the search term and page number
        //    var viewModel = new JobListViewModel
        //    {
        //        CurrentFilter = search,
        //        PageIndex = page,
        //        Jobs = await _jobRepository.GetPaginatedJobListAsync(search, page, pageSize)
        //    };

        //    // Calculate total number of jobs for pagination
        //    var totalJobs = await _jobRepository.GetTotalJobsAsync(search);
        //    viewModel.TotalPages = (int)Math.Ceiling((double)totalJobs / pageSize);
        //    viewModel.HasPreviousPage = (page > 1);
        //    viewModel.HasNextPage = (page < viewModel.TotalPages);

        //    return View(viewModel);
        //}


        //public async Task<IActionResult> Index()
        //{
        //    var jobs = await _jobRepository.GetAllJobsAsync();
        //    return View(jobs);
        //}

        //public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        //{
        //    // Retrieve paginated job listings from the database
        //    var allJobs =  await _jobRepository.GetAllJobsAsync();
        //    var jobs =allJobs// _dbContext.Jobs
        //        .OrderByDescending(j => j.PostedDate)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    // Calculate total number of jobs and pages
        //    var totalJobs = allJobs.Count;// _jobRepository.GetAllJobsAsync() .Jobs.Count();
        //    var totalPages = (int)Math.Ceiling((double)totalJobs / pageSize);

        //    // Pass the paginated jobs and pagination information to the view
        //    var model = new PaginatedList<Job>(jobs, page, pageSize, totalJobs, totalPages);
        //    return View(model);
        //}


        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            // Set the search term in ViewBag to display in the search bar
            ViewBag.SearchTerm = searchTerm;

            // Get all blog posts paginated
            var pageSize = 10; // Number of items per page
            var totalItems = await _jobRepository.GetTotalPostsCountAsync(searchTerm);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            var jobPosts = await _jobRepository.GetPostsAsync(searchTerm, page, pageSize);

            // Create a PaginatedList instance
            var paginatedList = new PaginatedList<Job>(jobPosts, page, pageSize, totalItems, totalPages);

            return View(paginatedList);
        }


        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);
           
            if (job == null)
                return NotFound();
            job.Applicants = (await _jobRepository.GetAllApplicantsAsync()).Where(a => a.JobId == job.Id).ToList();
            return View(job);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Apply(int jobId)
        {
            var job = await _jobRepository.GetJobByIdAsync(jobId);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> Apply(int jobId, IFormFile resume)
        {
            var job = await _jobRepository.GetJobByIdAsync(jobId);

            if (job == null)
            {
                return NotFound();
            }

            if (resume == null || resume.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a resume in PDF format.");
                return View(job);
            }

            // Check if the uploaded file is a PDF
            if (Path.GetExtension(resume.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError(string.Empty, "Please upload a resume in PDF format.");
                return View(job);
            }

            // Generate a unique filename
            var uniqueFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}{Path.GetExtension(resume.FileName)}";

            // Save the resume file to a location (you can modify this as per your requirements)
            var filePath = Path.Combine("wwwroot", "CV", uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resume.CopyToAsync(stream);
            }

            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            // Save the resume details to the database
            var newResume = new Resume
            {
                FileName = resume.FileName,
                FilePath = filePath,
                UploadDate = DateTime.Now
            };
           // await _jobRepository.CreateResumeAsync(newResume);

            // Create the applicant object and associate it with the job and the user
            var applicant = new Applicant
            {
                Name = currentUser.UserName, // Replace with the appropriate property from your ApplicationUser model
                Email = currentUser.Email, // Replace with the appropriate property from your ApplicationUser model
                Phone = currentUser.PhoneNumber==null? "No Phone": currentUser.PhoneNumber, // Replace with the appropriate property from your ApplicationUser model
                Resume = newResume,
                AppliedDate = DateTime.Now,
                JobId = jobId,
                ApplicantUserId = currentUser.Id // Replace with the appropriate property from your ApplicationUser model
            };
            await _jobRepository.CreateApplicantAsync(applicant);

            // Perform other logic, such as sending notifications

            // Set a success message notification
            TempData["ApplySuccess"] = "Resume uploaded successfully.";

            // Redirect to a thank you page or the job details page
            return RedirectToAction("Details", new { id = jobId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Job job)
        {
            if (job.Title!=null && job.Description!=null)
            {
                // Set the ApplicationUser as the PostedByUser
                var currentUser = await _userManager.GetUserAsync(User);
                job.PostedByUser = currentUser;

                job.PostedDate = DateTime.Now;

                await _jobRepository.CreateJobAsync(job);
                return RedirectToAction("Index", "Job");
            }

            return View(job);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();

            return View(job);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Job job)
        {
            if (job.Title != null && job.Description != null)
            {
                var existingJob = await _jobRepository.GetJobByIdAsync(id);
                if (existingJob == null)
                    return NotFound();

                existingJob.Title = job.Title;
                existingJob.Description = job.Description;

                await _jobRepository.UpdateJobAsync(existingJob);
                return RedirectToAction("Index");
            }

            return View(job);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);
            if (job == null)
                return NotFound();

            await _jobRepository.DeleteJobAsync(job);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Applicants(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            var applicants = (await _jobRepository.GetAllApplicantsAsync()).Where(a => a.JobId == job.Id).ToList();
            
            return View(applicants);
        }

        public async Task<IActionResult>
            DownloadResume(int resumeId)
        {
            var resume = await _jobRepository.GetResumeByIdAsync(resumeId);
            if (resume == null)
            {
                return NotFound();
            }
           
            var fileBytes = System.IO.File.ReadAllBytes(resume.FilePath);
            return File(fileBytes, "application/pdf", resume.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveJob(int Id)
        {
            var job = await _jobRepository.GetJobByIdAsync(Id);
            if (job == null)
            {
                return NotFound();
            }

            job.IsApproved = true;
            await _jobRepository.UpdateJobAsync(job);

            // Redirect back to the ActorsList action after approval
            return RedirectToAction(nameof(Index));
        }


    }
}
