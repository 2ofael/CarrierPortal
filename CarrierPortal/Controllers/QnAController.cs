using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Claims;

namespace CarrierPortal.Controllers
{
    public class QnAController : Controller
    {
        private readonly IQnARepository _qnaRepository;

        public QnAController(IQnARepository qnaRepository)
        {
            _qnaRepository = qnaRepository;
        }

        // GET: Questions
        //public async Task<IActionResult> Index()
        //{
        //    var questions = await _qnaRepository.GetAllQuestionsAsync();
        //    return View(questions);
        //}
        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            // Set the search term in ViewBag to display in the search bar
            ViewBag.SearchTerm = searchTerm;

            // Get all blog posts paginated
            var pageSize = 10; // Number of items per page
            var totalItems = await _qnaRepository.GetTotalPostsCountAsync(searchTerm);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            var QnAPosts = await _qnaRepository.GetPostsAsync(searchTerm, page, pageSize);

            // Create a PaginatedList instance
            var paginatedList = new PaginatedList<Question>(QnAPosts, page, pageSize, totalItems, totalPages);

            return View(paginatedList);
        }


        // GET: Questions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var question = await _qnaRepository.GetQuestionByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            var answers = await _qnaRepository.GetAnswersForQuestionAsync(id);
            question.Answers = answers;

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionViewModel question)
        {
            if (ModelState.IsValid)
            {
                Question newQuestion = new Question
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Title = question.Title,
                    Content = question.Content,
                    CreationDate = DateTime.Now,
                    IsApproved = false,
                    Votes = 0
                };

            
              
                await _qnaRepository.CreateQuestionAsync(newQuestion);
                return RedirectToAction(nameof(Index));
            }

            return View(question);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var question = await _qnaRepository.GetQuestionByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        //// POST: Questions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (question.Content!=null && question.Title!=null)
            {
                var PrevQuestion =await _qnaRepository.GetQuestionByIdAsync(id);
                PrevQuestion.Title = question.Title;
                PrevQuestion.Content = question.Content;

                await _qnaRepository.UpdateQuestionAsync(PrevQuestion);
                return RedirectToAction(nameof(Details),new {id = id});
            }

            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var question = await _qnaRepository.GetQuestionByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        //// POST: Questions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var question = await _qnaRepository.GetQuestionByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            await _qnaRepository.DeleteQuestionAsync(question);
            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    var question = await _qnaRepository.GetQuestionByIdAsync(id);

        //    if (question == null)
        //    {
        //        return NotFound();
        //    }

        //    var viewModel = new QuestionViewModel
        //    {
        //        Id = question.Id,
        //        Title = question.Title,
        //        Content = question.Content
        //    };

        //    return View(viewModel);
        //}

        //// POST: Questions/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, QuestionViewModel question)
        //{
        //    if (id != question.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        Question existingQuestion = await _qnaRepository.GetQuestionByIdAsync(id);
        //        if (existingQuestion == null)
        //        {
        //            return NotFound();
        //        }

        //        existingQuestion.Title = question.Title;
        //        existingQuestion.Content = question.Content;

        //        await _qnaRepository.UpdateQuestionAsync(existingQuestion);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(question);
        //}

        [HttpPost]
     
        public async Task<IActionResult> Upvote(string questionId)
        {
            var question = await _qnaRepository.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                return NotFound();
            }

            // Perform upvote logic here
            question.Votes++;

            // Save the updated question
            await _qnaRepository.UpdateQuestionAsync(question);

            // Return the updated vote count
            return Json(new { voteCount = question.Votes });
        }
        
      
        [HttpPost]
       

        public async Task<IActionResult> Downvote(string questionId)
        {
            var question = await _qnaRepository.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                return NotFound();
            }

            // Perform downvote logic here
            question.Votes--;

            // Save the updated question
            await _qnaRepository.UpdateQuestionAsync(question);

            // Return the updated vote count
            return Json(new { voteCount = question.Votes });
        }
        [HttpPost]
        public async Task<IActionResult> hello(string Id)
        {
            return Json("hello "+Id);
        }


    }



}
