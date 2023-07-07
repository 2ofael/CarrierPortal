using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarrierPortal.Controllers
{
    public class AnswerController : Controller
    {
        private readonly IQnARepository _qnaRepository;

        public AnswerController(IQnARepository qnaRepository)
        {
            _qnaRepository = qnaRepository;
        }

        // GET: Answer/Create
        public IActionResult Create(string questionId)
        {
            var viewModel = new AnswerViewModel
            {
                QuestionId = questionId
            };
            return View(viewModel);
        }

        //// POST: Answer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnswerViewModel answer)
        {
            if (answer.Content!=null && answer.QuestionId!=null)
            {
                Answer newAnswer = new Answer
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    QuestionId = answer.QuestionId,
                    Content = answer.Content,
                    CreationDate = DateTime.Now,
                    IsApproved = false,
                    Votes = 0
                };

                await _qnaRepository.CreateAnswerAsync(newAnswer);
                return RedirectToAction("Details", "QnA", new { id = answer.QuestionId });
            }

            return View(answer);
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            

            var answer = await _qnaRepository.GetAnswerByIdAsync(id);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }
    }

    //// GET: Answer/Edit/5
    //public async Task<IActionResult> Edit(string id)
    //{
    //    if (string.IsNullOrEmpty(id))
    //    {
    //        return NotFound();
    //    }

    //    var answer = await _qnaRepository.GetAnswerByIdAsync(id);

    //    if (answer == null)
    //    {
    //        return NotFound();
    //    }

    //    var viewModel = new AnswerViewModel
    //    {
    //        Id = answer.Id,
    //        Content = answer.Content
    //    };

    //    return View(viewModel);
    //}

    //// POST: Answer/Edit/5
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(string id, AnswerViewModel answer)
    //{
    //    if (id != answer.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        Answer existingAnswer = await _qnaRepository.GetAnswerByIdAsync(id);
    //        if (existingAnswer == null)
    //        {
    //            return NotFound();
    //        }

    //        existingAnswer.Content = answer.Content;

    //        await _qnaRepository.UpdateAnswerAsync(existingAnswer);
    //        return RedirectToAction("Details", "QnA", new { id = existingAnswer.QuestionId });
    //    }

    //    return View(answer);
    //}

    //// GET: Answer/Delete/5
    //public async Task<IActionResult> Delete(string id)
    //{
    //    if (string.IsNullOrEmpty(id))
    //    {
    //        return NotFound();
    //    }

    //    var answer = await _qnaRepository.GetAnswerByIdAsync(id);

    //    if (answer == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(answer);
    //}

    //// POST: Answer/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(string id)
    //{
    //    var answer = await _qnaRepository.GetAnswerByIdAsync(id);

    //    if (answer == null)
    //    {
    //        return NotFound();
    //    }

    //    string questionId = answer.QuestionId;

    //    await _qnaRepository.DeleteAnswerAsync(answer);
    //    return RedirectToAction("Details", "QnA", new { id = questionId });
    //}
}
}
