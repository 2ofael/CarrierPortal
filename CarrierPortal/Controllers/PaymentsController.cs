﻿using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.Services.EmailServices;
using CarrierPortal.ViewModels;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CarrierPortal.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
       // public static string TempEmail;
        //public string actorId;
      //  public static Actor RequestedActor;
        private readonly IActorRepository _actorRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentsController(IHttpContextAccessor httpContextAccessor,AppDbContext appDbContext, IActorRepository actorRepository, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {

            _actorRepository = actorRepository;
            _userManager = userManager;
            _emailService = emailService;
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;

        }



        public IActionResult Index(string mentorId)
        {
           // RequestedActor = await _actorRepository.GetActorById(mentorId);
            //TempEmail = (await _userManager.GetUserAsync(User)).Email;

            HttpContext.Session.SetString("mentorId",mentorId);

            return View();
        }




        [HttpPost]
        public IActionResult CreatePaymentIntent()
        {
            StripeConfiguration.ApiKey = "sk_test_51Neu2iIuE0SL7TqEtXVVnRTiZ5uHAsJgZBsMl9oUE8TZQeGUqcZ6xPBKXS9b3g23I2suY3NopmaC8GUc365x6CHU005yfcGuf6"; // Replace with your actual Stripe secret key

            var options = new PaymentIntentCreateOptions
            {
                Amount = 100, // Amount in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string>
        {
            "card",
        },
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Json(new { ClientSecret = paymentIntent.ClientSecret });
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] ProcessPaymentModel model)
        {
            StripeConfiguration.ApiKey = "sk_test_51Neu2iIuE0SL7TqEtXVVnRTiZ5uHAsJgZBsMl9oUE8TZQeGUqcZ6xPBKXS9b3g23I2suY3NopmaC8GUc365x6CHU005yfcGuf6"; // Replace with your Stripe secret key

            var service = new PaymentIntentService();
            var paymentIntent = service.Get(model.PaymentIntentId);

            try
            {
                if (paymentIntent.Status == "succeeded")
                {
                    SendInfo();
                    // RedirectToAction(nameof(PaymentSuccessfull));
                    // Payment already succeeded, return a JSON response
                    // _emailService.SendEmailAsync(TempEmail,"Mentor Info", RequestedActor.ActorName );
                    return Json(new { success = true });
                }
                else if (paymentIntent.Status == "requires_payment_method")
                {
                    // Attempt to confirm the payment again
                    paymentIntent = service.Confirm(paymentIntent.Id);

                    if (paymentIntent.Status == "succeeded")
                    {
                        SendInfo();
                        // return RedirectToAction(nameof(PaymentSuccessfull));
                        //_emailService.SendEmailAsync("tofrom@gmail.com", "hello", "<h1>Ok</h1>");
                        // Payment successfully confirmed, return a JSON response
                        return Json(new { success = true });
                    }
                    else
                    {
                        // RedirectToAction(nameof(PaymentError));
                        // Payment confirmation failed, return a JSON response
                        return Json(new { success = false });
                    }
                }
                else
                {
                    //  return RedirectToAction(nameof(PaymentError));
                    // Payment failed, return a JSON response
                    return Json(new { success = false });
                }
            }
            catch (StripeException ex)
            {
                // Handle any Stripe API exceptions
                return Json(new { success = false, error = ex.Message });
            }
        }


        private async void SendInfo()
        {
            string emailBody =  GenerateActorProfileEmail();
            //var currUser= await _userManager.GetUserAsync(User);
            string tempEmail = _httpContextAccessor.HttpContext.User.Identity.Name; //currUser.Email;
            // Send the email using your email service
          await  _emailService.SendEmailAsync(tempEmail, "Your Mentor Profile", emailBody);
        }



        private string GenerateActorProfileEmail()
        {
            var emailTemplate = System.IO.File.ReadAllText("EmailTemplates/ActorProfileEmail.html");

            string mentorId = HttpContext.Session.GetString("mentorId");

            Actor mentor =  _appDbContext.Actors.Where(a => a.ActorId == mentorId).FirstOrDefault();

            emailTemplate = emailTemplate.Replace("{ActorName}", mentor.ActorName)
                                         .Replace("{Skills}", mentor.Skills)
                                         .Replace("{CurrentProfession}", mentor.CurrentProfession)
                                         .Replace("{AcademicQualification}", mentor.AcademicQualification)
                                         .Replace("{Email}", mentor.Email)
                                         .Replace("{Phone}", mentor.Phone);
            if (mentor.isMentor == false)
                emailTemplate = String.Empty;

            return emailTemplate;
        }

        [HttpPost]
        public IActionResult PaymentSuccessfull()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PaymentError()
        {
            return View();
        }


    }


}