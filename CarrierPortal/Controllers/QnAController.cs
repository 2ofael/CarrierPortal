using CarrierPortal.EmailTemplates;
using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Linq;
using System.Security.Claims;

namespace CarrierPortal.Controllers
{
    public class QnAController : Controller
    {
        private readonly IQnARepository _qnaRepository;
        private readonly ActionMessageSender _ActionMessageSender;

        public QnAController(IQnARepository qnaRepository,ActionMessageSender actionMessageSender)
        {
            _qnaRepository = qnaRepository;
            _ActionMessageSender = actionMessageSender;
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
                TempData["isCreated"] = true;
                return RedirectToAction(nameof(Details), new {id= newQuestion.Id });
            }
            TempData["isCreated"] = false;
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
                PrevQuestion.IsApproved = false;

                await _qnaRepository.UpdateQuestionAsync(PrevQuestion);
                TempData["isEdited"] = true;
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
            TempData["isDeleted"] = true;
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
        public async Task<IActionResult> ApproveQuestion(string Id)
        {
            var question = await _qnaRepository.GetQuestionByIdAsync(Id);
            if (question == null)
            {
                return NotFound();
            }

            question.IsApproved = true;
            await _qnaRepository.UpdateQuestionAsync(question);
            TempData["isApproved"] = true;
            // Redirect back to the ActorsList action after approval
            var url = Url.Action("Details", "QnA", new { id = Id },Request.Scheme);


           await _ActionMessageSender.SendActionMessage(question.UserId, url);


            return RedirectToAction(nameof(Details), new { id = Id });
        }



        [HttpPost]
        public async Task<IActionResult> UpvoteQuestion(string questionId)
        {
            var userId = GetCurrentUserId();
            var question = await _qnaRepository.GetQuestionByIdAsync(questionId);

            if (question == null)
            {
                return NotFound();
            }

            if (!HasUserVotedQuestion(userId, questionId))
            {
                question.Votes++;
                await _qnaRepository.UpdateQuestionAsync(question);

                var vote = new QuestionVote { QuestionId = questionId, UserId = userId, IsUpvote = true };
                await _qnaRepository.CreateQuestionVoteAsync(vote);
            }

            return RedirectToAction(nameof(Details), new { id = questionId });
        }

        [HttpPost]
        public async Task<IActionResult> DownvoteQuestion(string questionId)
        {
            var userId = GetCurrentUserId();
            var question = await _qnaRepository.GetQuestionByIdAsync(questionId);

            if (question == null)
            {
                return NotFound();
            }

            if (!HasUserVotedQuestion(userId, questionId))
            {
                question.Votes--;
                await _qnaRepository.UpdateQuestionAsync(question);

                var vote = new QuestionVote { QuestionId = questionId, UserId = userId, IsUpvote = false };
                await _qnaRepository.CreateQuestionVoteAsync(vote);
            }

            return RedirectToAction(nameof(Details), new { id = questionId });
        }

        [HttpPost]
        public async Task<IActionResult> UpvoteAnswer(string answerId)
        {
            var userId = GetCurrentUserId();
            var answer = await _qnaRepository.GetAnswerByIdAsync(answerId);

            if (answer == null)
            {
                return NotFound();
            }

            if (!HasUserVotedAnswer(userId, answerId))
            {
                answer.Votes++;
                await _qnaRepository.UpdateAnswerAsync(answer);

                var vote = new AnswerVote { AnswerId = answerId, UserId = userId, IsUpvote = true };
                await _qnaRepository.CreateAnswerVoteAsync(vote);
            }

            return RedirectToAction("Details","Answer", new { Id = answer.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DownvoteAnswer(string answerId)
        {
            var userId = GetCurrentUserId();
            var answer = await _qnaRepository.GetAnswerByIdAsync(answerId);

            if (answer == null)
            {
                return NotFound();
            }

            if (!HasUserVotedAnswer(userId, answerId))
            {
                answer.Votes--;
                await _qnaRepository.UpdateAnswerAsync(answer);

                var vote = new AnswerVote { AnswerId = answerId, UserId = userId, IsUpvote = false };
                await _qnaRepository.CreateAnswerVoteAsync(vote);
            }
            return RedirectToAction("Details", "Answer", new { Id = answer.Id });

           // return RedirectToAction("QuestionDetails", new { questionId = answer.QuestionId });
        }

        private bool HasUserVotedQuestion(string userId, string questionId)
        {
            return _qnaRepository.HasUserVotedQuestion(userId, questionId);
        }

        private bool HasUserVotedAnswer(string userId, string answerId)
        {
            return _qnaRepository.HasUserVotedAnswer(userId, answerId);
        }

        private string GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }


        public async Task<IActionResult> QuestionAskedByYou()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var questionsAskedByUser = (await _qnaRepository.GetAllQuestionsAsync()).Where(q => q.UserId == userId).ToList();
          
            return View(questionsAskedByUser);
        }



        public async Task<IActionResult> AnsweredByUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var answeredByUser =  (await _qnaRepository.GetAllAnswersAsync()).Where(a => a.UserId == userId).ToList();

            return View(answeredByUser);
        }




    }



}
