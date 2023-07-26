﻿using CarrierPortal.AppViewModel;
using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.Services.PhotoServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Numerics;
using System.Security.Claims;

namespace CarrierPortal.Controllers
{
    public class MentorController : Controller
    {
        private readonly IGenericRepository<Actor> _repository;
        private readonly IPhotoService _photoService;
        private readonly IPhotoListService _photoListService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext appDbContext;
        private readonly IActorRepository _actorRepository;

        public MentorController(IActorRepository actorRepository, AppDbContext appDbContext, UserManager<ApplicationUser>userManager, IGenericRepository<Actor> repository, IPhotoService photoService,IPhotoListService photoListService)
        {
            _repository = repository;
            _photoService = photoService;
            _photoListService= photoListService;
            _userManager = userManager;
            this.appDbContext = appDbContext;
            _actorRepository = actorRepository;
        }



        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            // Set the search term in ViewBag to display in the search bar
            ViewBag.SearchTerm = searchTerm;

            // Get all blog posts paginated
            var pageSize = 10; // Number of items per page
            var totalItems = await _actorRepository.GetTotalPostsCountAsync(searchTerm);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            var ActorPosts = await _actorRepository.GetPostsAsync(searchTerm, page, pageSize);

            // Create a PaginatedList instance
            var paginatedList = new PaginatedList<Actor>(ActorPosts, page, pageSize, totalItems, totalPages);

            return View(paginatedList);
        }




        [HttpGet]
        public async Task<IActionResult> ViewProfile(string actorId)
        {

            var actor = await _actorRepository.GetActorById(actorId);
            return View(actor);

        }



        [HttpGet]
        public IActionResult ApplyAsMentor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyAsMentor(MentorApplication mentorApplication)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.Users
           .Include(u => u.Mentor) // Include the mentor property
           .FirstOrDefaultAsync(u => u.Id == userId);

            if (ModelState.IsValid)
            {
                
                if (CurrentUser.Mentor == null)
                {

                    var newMentor = new Actor
                    {

                        User = CurrentUser,

                        ActorId = Guid.NewGuid().ToString(),
                        ActorName = mentorApplication.ActorName,
                        About = mentorApplication.About,
                        Address = mentorApplication.Address,
                        age = mentorApplication.age,
                        Skills = mentorApplication.Skills,
                        AcademicQualification = mentorApplication.AcademicQualification,
                        CurrentProfession = mentorApplication.CurrentProfession,
                        Gender = mentorApplication.Gender,
                        Email = mentorApplication.Email,
                        Phone = mentorApplication.Phone,




                        ProfilePhoto = _photoService.SavePhoto(mentorApplication.ProfilePhoto, "Photo"),
                        cv = _photoService.SavePhoto(mentorApplication.cv, "CV"),
                        Certificates = _photoService.SavePhoto(mentorApplication.Certificates, "Certificates")

                    };




                   await _actorRepository.AddActor(newMentor);

                }
                else
                {


                  var  CurrMentor =await _actorRepository.GetActorById(CurrentUser.Mentor.ActorId);

                    //CurrMentor.ActorId = Guid.NewGuid().ToString(),
                    CurrMentor.ActorName = mentorApplication.ActorName;
                    CurrMentor.About = mentorApplication.About;
                    CurrMentor.Address = mentorApplication.Address;
                    CurrMentor.age = mentorApplication.age;
                    CurrMentor.Skills = mentorApplication.Skills;
                    CurrMentor.AcademicQualification = mentorApplication.AcademicQualification;
                    CurrMentor.CurrentProfession = mentorApplication.CurrentProfession;
                    CurrMentor.Gender = mentorApplication.Gender;
                    CurrMentor.Email = mentorApplication.Email;
                    CurrMentor.Phone = mentorApplication.Phone;




                    CurrMentor.ProfilePhoto = _photoService.SavePhoto(mentorApplication.ProfilePhoto, "Photo");
                    CurrMentor.cv = _photoService.SavePhoto(mentorApplication.cv, "CV");
                    CurrMentor.Certificates = _photoService.SavePhoto(mentorApplication.Certificates, "Certificates");

                   await _actorRepository.UpdateActor(CurrMentor);


                }




            }
            
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ApproveActor(string actorId)
        {
            var actor = await _actorRepository.GetActorById(actorId);
            if (actor == null)
            {
                return NotFound();
            }

            actor.isMentor = true;
            await _actorRepository.UpdateActor(actor);

            // Redirect back to the ActorsList action after approval
            return RedirectToAction("ActorsList");
        }


        [HttpPost]
        public async Task<IActionResult> LovePost(string ActorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Implement a method to get the current logged-in user's Id
            var post = await _actorRepository.GetActorById(ActorId);
            var isLoved = post.Loved.Select(l => l.UserNameIdentifier == userId).FirstOrDefault();
            if (post != null)
            {
                if (isLoved)
                {
                    var curLoved = post.Loved.Select(l => l).Where(l => l.UserNameIdentifier == userId).FirstOrDefault();
                    post.Loved.Remove(curLoved); // Remove the user's Id from the Loved list
                    post.Votes--; // Decrease the total number of votes/loves for the post
                }
                else
                {
                    post.Loved.Add(new ActorLoved { UserNameIdentifier = userId }); // Add the user's Id to the Loved list
                    post.Votes++; // Increment the total number of votes/loves for the post
                }

                await _actorRepository.UpdateActor(post);
            }

            return RedirectToAction("ViewProfile", new { actorId = ActorId });
        }

    }
}
