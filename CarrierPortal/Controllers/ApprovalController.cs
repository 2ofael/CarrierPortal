using CarrierPortal.Models.DataModel;
using CarrierPortal.Models;
using CarrierPortal.Repository;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Anssi;

namespace CarrierPortal.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IActorRepository _actorRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IQnARepository _qnaRepository;
        private readonly AppDbContext _appDbContext;

        public ApprovalController(IActorRepository actorRepository,
            IBlogRepository blogRepository,
            IJobRepository jobRepository,
            IQnARepository qnARepository,
            AppDbContext appDbContext
            )
        {
            _actorRepository = actorRepository;
            _blogRepository = blogRepository;
            _jobRepository = jobRepository;
            _qnaRepository = qnARepository;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
     
       public async Task<IActionResult> ApproveMentor(int page = 1)
        {
            List<Actor> actors = (await _actorRepository.GetAllActors()).Where(m=>m.isSubscribed == true && m.isMentor == false).ToList();

            var pageSize = 10; // Number of items per page
            var totalItems = actors.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }
            var paginatedList = new PaginatedList<Actor>(actors, page, pageSize, totalItems, totalPages);

            return View(paginatedList);

        }



        public async Task<IActionResult> ApproveBlog(int page = 1)
        {
            List<BlogPost> blogs = (await _blogRepository.GetAllPostsAsync()).Where(b => b.IsApproved == false).ToList();

            var pageSize = 10; // Number of items per page
            var totalItems = blogs.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }
            var paginatedList = new PaginatedList<BlogPost>(blogs, page, pageSize, totalItems, totalPages);

            return View(paginatedList);

        }

        public async Task<IActionResult> ApproveQnA(int page = 1)
        {
            List<Question> questions = (await _qnaRepository.GetAllQuestionsAsync()).Where(b => b.IsApproved == false).ToList();

            var pageSize = 10; // Number of items per page
            var totalItems = questions.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }
            var paginatedList = new PaginatedList<Question>(questions, page, pageSize, totalItems, totalPages);

            return View(paginatedList);

        }

        public async Task<IActionResult> ApproveJob(int page = 1)
        {
            List<Job> jobs = (await _jobRepository.GetAllJobsAsync()).Where(b => b.IsApproved == false).ToList();

            var pageSize = 10; // Number of items per page
            var totalItems = jobs.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }
            var paginatedList = new PaginatedList<Job>(jobs, page, pageSize, totalItems, totalPages);

            return View(paginatedList);

        }

        public async Task<IActionResult> ApproveAnswer(int page = 1)
        {
            List<Answer> answer = _appDbContext.Answers.Where(b => b.IsApproved == false).ToList();

            var pageSize = 10; // Number of items per page
            var totalItems = answer.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }
            var paginatedList = new PaginatedList<Answer>(answer, page, pageSize, totalItems, totalPages);

            return View(paginatedList);

        }








    }
}
