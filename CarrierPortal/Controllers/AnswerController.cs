

using CarrierPortal.EmailTemplates;
using CarrierPortal.Extensions;
using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.Services.EmailServices;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CarrierPortal.Controllers
{
   
    public class AnswerController : Controller
    {
     
        private readonly IQnARepository _qnaRepository;
        private readonly ActionMessageSender _ActionMessageSender;
        private readonly IEmailService _emailService;

        public AnswerController(IQnARepository qnaRepository,
            ActionMessageSender actionMessageSender, IEmailService emailService
            
         )
        {
           
            _qnaRepository = qnaRepository;
            _ActionMessageSender = actionMessageSender;
            _emailService = emailService;
            
        }

        // GET: Answer/Create
        [Authorize(Roles = "Mentor")]
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
            if (answer.Content != null && answer.QuestionId != null)
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


                TempData["isCreated"] = true;
                await _qnaRepository.CreateAnswerAsync(newAnswer);
                string userEmail = (await _qnaRepository.GetQuestionByIdAsync(answer.QuestionId)).User.Email;
                var url = Url.Action("Details", "Answer", new { id = newAnswer.Id }, Request.Scheme);
                string body = $"<h2>Your new answer <a href=\"{url}\">Link</a></h2>";


                _emailService.SendEmailAsync(userEmail, "You have new Answer", body);

                return RedirectToAction("Details", "Answer", new { id = newAnswer.Id });
                
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


        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> Edit(string id)
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

            var viewModel = new AnswerViewModel
            {
                Id = answer.Id,
                Content = answer.Content
            };

            return View(viewModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AnswerViewModel answer)
        {
            if (id != answer.Id)
            {
                return NotFound();
            }

            if (answer.Content != null && answer.Id != null)
            {
                
                Answer existingAnswer = await _qnaRepository.GetAnswerByIdAsync(id);
                if (existingAnswer == null)
                {
                    return NotFound();
                }

                existingAnswer.Content = answer.Content;
                existingAnswer.IsApproved = false;

                if(!UserHelper.IsCurrentUserOrAdmin(existingAnswer.Id, User))
                {
                    return BadRequest();
                }

                await _qnaRepository.UpdateAnswerAsync(existingAnswer);
                TempData["isEdited"] = true;
                return RedirectToAction("Details", "Answer", new { id = existingAnswer.Id });
            }


            return View(answer);
        }

        //// GET: Answer/Delete/5
        [Authorize(Roles = "Mentor")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var answer = await _qnaRepository.GetAnswerByIdAsync(id);

            if (!UserHelper.IsCurrentUserOrAdmin(answer.UserId, User))
            {
                return BadRequest();
            }

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // POST: Answer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var answer = await _qnaRepository.GetAnswerByIdAsync(id);

            if (answer == null)
            {
                return NotFound();
            }

            string questionId = answer.QuestionId;

            if (!UserHelper.IsCurrentUserOrAdmin(answer.UserId, User))
            {
                return BadRequest();
            }

            await _qnaRepository.DeleteAnswerAsync(answer);
            TempData["isDeleted"] = true;
            return RedirectToAction("Details", "QnA", new { id = questionId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ApproveAnswer(string Id)
        {
            var ans = await _qnaRepository.GetAnswerByIdAsync(Id);
            if (ans == null)
            {
                return NotFound();
            }

            ans.IsApproved = true;
            await _qnaRepository.UpdateAnswerAsync(ans);

            // Redirect back to the ActorsList action after approval
            TempData["isApproved"] = true;
            var url = Url.Action("Details", "Answer", new { id = Id }, Request.Scheme);

            Response.OnCompleted(async () =>
            {
                _ActionMessageSender.SendActionMessage(ans.UserId, url);
            });
            return RedirectToAction(nameof(Details), new { id = Id });  
         
        }

    }
}
