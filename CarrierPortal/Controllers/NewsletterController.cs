using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Services.EmailServices;
using CarrierPortal.Services.PhotoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Controllers
{
    [Authorize]
    public class NewsletterController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        private IPhotoService _photoService;

        public NewsletterController(IPhotoService photoService, IEmailService emailService, UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _emailService = emailService;
            _photoService = photoService;
        }

        // GET: /newsletter/subscribe
        [HttpGet]
        public async  Task<IActionResult> Subscribe()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool IsSubscribed =  _dbContext.NewsletterSubscriptions.Any(u=>u.Email==currentUser.Email);




            return View(IsSubscribed);
        }

        // POST: /newsletter/subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribed()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var email = currentUser.Email;

            var subscription = new NewsletterSubscription { Email = email };
            _dbContext.NewsletterSubscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Subscribe");

            // Redirect to a success page or show a success message
         
        }

        // POST: /newsletter/unsubscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsubscribe()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var email = currentUser.Email;

            var subscription = await _dbContext.NewsletterSubscriptions.FirstOrDefaultAsync(s => s.Email == email);
            if (subscription != null)
            {
                _dbContext.NewsletterSubscriptions.Remove(subscription);
                await _dbContext.SaveChangesAsync();
            }

            // Redirect to a success page or show a success message
            return RedirectToAction("Subscribe");
        }

        // GET: /newsletter/success
        public IActionResult SubscribeSuccess()
        {
            return View();
        }

        // GET: /newsletter/unsubscribe-success
        public IActionResult UnsubscribeSuccess()
        {
            return View();
        }

        public IActionResult Send()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(IFormFile pdfFile)
        {
            if (pdfFile != null && pdfFile.Length > 0)
            {

                List<string>allEmails = await _dbContext.NewsletterSubscriptions.Select(u=>u.Email).ToListAsync();
                string FileName =  _photoService.SavePhoto(pdfFile, "CV" ,true);
                foreach(string email in allEmails)
                {

                    await _emailService.SendEmailAsync(email, "Career News of the month",

                        "File attached", FileName
                        );
                }


                return RedirectToAction("SendSuccess");
            }


            return View();
            // Redirect to a success page or show a success message
           
        }
        [HttpGet]
        public IActionResult SendSuccess()
        {
            return View();
        }


    }
}

