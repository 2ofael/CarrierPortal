using CarrierPortal.EmailTemplates;
using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CarrierPortal.Controllers
{
   
    public class JobController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly ActionMessageSender _ActionMessageSender;

        public JobController(ActionMessageSender actionMessageSender, AppDbContext appDbContext, IJobRepository jobRepository, UserManager<ApplicationUser> userManager)
        {
            _jobRepository = jobRepository;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _ActionMessageSender = actionMessageSender;
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

                int Id =  await _jobRepository.CreateJobAsync(job);
                TempData["isJobCreated"] = true;
                return RedirectToAction(nameof(Details), new {id = Id });
            }
            TempData["isJobCreated"] = false;
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
                existingJob.IsApproved = false;
                await _jobRepository.UpdateJobAsync(existingJob);
                TempData["isEdited"] = true;
                return RedirectToAction(nameof(Details), new {id=id});
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
            TempData["IsDeleted"] = true;
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
            TempData["isApproved"] = true;
            var url = Url.Action("Details", "Job", new { id = Id }, Request.Scheme);


            await _ActionMessageSender.SendActionMessage(job.PostedByUserId, url);
            // Redirect back to the ActorsList action after approval
            return RedirectToAction(nameof(Details), new { id = Id });
        }
        public async Task<IActionResult> Filter(string searchTerm="", int page = 1)
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

            return View(new JobAndPagination { Paginations=paginatedList,Filter=new JobFilterModel()});
        }

        [HttpPost]
        public IActionResult Filter(JobFilterModel jobFilter, int page = 1)
        {
            const int pageSize = 10; // Number of items per page

            IQueryable<Job> filteredActors = _appDbContext.Jobs.Include(j=>j.Applicants); // Assuming you have an Actor DbSet in ApplicationDbContext

            // Apply filtering based on the form inputs
            if (!string.IsNullOrEmpty(jobFilter.Title))
                filteredActors = filteredActors.Where(a => a.Title.ToLower().Contains(jobFilter.Title.ToLower()));

            //if (!string.IsNullOrEmpty(mentorFilter.Skills))
            //    filteredActors = filteredActors.Where(a => a.Skills.Contains(mentorFilter.Skills));

            //if (!string.IsNullOrEmpty(mentorFilter.Gender))
            //    filteredActors = filteredActors.Where(a => a.Gender == mentorFilter.Gender);


            //if (!string.IsNullOrEmpty(mentorFilter.Gender))
            //    filteredActors = filteredActors.Where(a => a.Gender == mentorFilter.Gender);


            //if (!string.IsNullOrEmpty(mentorFilter.CurrentProfession))
            //    filteredActors = filteredActors.Where(a => a.CurrentProfession == mentorFilter.CurrentProfession);


            //if (!string.IsNullOrEmpty(mentorFilter.AcademicQualification))
            //    filteredActors = filteredActors.Where(a => a.AcademicQualification == mentorFilter.AcademicQualification);
            if (!string.IsNullOrEmpty(jobFilter.Description))
                filteredActors = filteredActors.Where(a => a.Description.ToLower().Contains(jobFilter.Description.ToLower()));

            if (!string.IsNullOrEmpty(jobFilter.Location))
                filteredActors = filteredActors.Where(a => a.Location.ToLower() == jobFilter.Location.ToLower());

            if (jobFilter.MaxSalary!=0 && jobFilter.MinSalary!=0)
                filteredActors = filteredActors.Where(j => j.Salary<= jobFilter.MaxSalary && j.Salary>=jobFilter.MinSalary);

            if (jobFilter.PostedAfterDate !=null)
                filteredActors = filteredActors.Where(j => j.PostedDate>=jobFilter.PostedAfterDate);
            if (jobFilter.MaxApplicants >= 0 && jobFilter.MinApplicants >= 0) ;
                filteredActors = filteredActors.Where(j => j.Applicants.Count()>=jobFilter.MinApplicants && j.Applicants.Count()<=jobFilter.MaxApplicants);

            int totalItems = filteredActors.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Apply pagination using PaginatedList<T>
            List<Job> pagedActors = filteredActors.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            // pagedActors = pagedActors.Where(a => a.age >= mentorFilter.age && a.age <= mentorFilter.age).ToList();
            var paginatedList = new PaginatedList<Job>(pagedActors, page, pageSize, totalItems, totalPages);

            return View(new JobAndPagination { Paginations = paginatedList , Filter = jobFilter });
        }


        public async Task<IActionResult> AllJobPostedByYou()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobsByUser = (await _jobRepository.GetAllJobsAsync()).Where(j=>j.PostedByUserId==userId).ToList();  



            return View(jobsByUser);
        }
        public async Task<IActionResult> AppliedByYou()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobsAppliedByUser = (await _jobRepository.GetAllJobsAsync()).Where(j => j.Applicants.Any(a=>a.ApplicantUserId==userId)).ToList();



            return View(jobsAppliedByUser);
        }


    }
}
