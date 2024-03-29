﻿using CarrierPortal.AppViewModel;
using CarrierPortal.EmailTemplates;
using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using CarrierPortal.Services.PhotoServices;
using CarrierPortal.ViewModels;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Claims;

namespace CarrierPortal.Controllers
{
    [Authorize(Roles ="MentorShip")]
    public class MentorController : Controller
    {
        private readonly IGenericRepository<Actor> _repository;
        private readonly IPhotoService _photoService;
        private readonly IPhotoListService _photoListService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext appDbContext;
        private readonly IActorRepository _actorRepository;
        private readonly ActionMessageSender _ActionMessageSender;

        public MentorController(ActionMessageSender actionMessageSender, IActorRepository actorRepository, AppDbContext appDbContext, UserManager<ApplicationUser>userManager, IGenericRepository<Actor> repository, IPhotoService photoService,IPhotoListService photoListService)
        {
            _repository = repository;
            _photoService = photoService;
            _photoListService= photoListService;
            _userManager = userManager;
            this.appDbContext = appDbContext;
            _actorRepository = actorRepository;
            _ActionMessageSender = actionMessageSender;

           
            
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




        [Route("/Mentor/ViewProfile/{actorId}")]
        public async Task<IActionResult> ViewProfile( string actorId)
        {
            
                if (ModelState.IsValid)
                {
                    var actor = await _actorRepository.GetActorById(actorId);

                    if (actor == null)
                    {
                        return NotFound();
                    }


                    return View(actor);
                }
            
            
          
            return NotFound();

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
                        DateOfBirth = mentorApplication.DateOfBirth,
                        Skills = mentorApplication.Skills,
                        AcademicQualification = mentorApplication.AcademicQualification,
                        CurrentProfession = mentorApplication.CurrentProfession,
                        Gender = mentorApplication.Gender,
                        Email = mentorApplication.Email,
                        Phone = mentorApplication.Phone,
                        isSubscribed = true,




                    ProfilePhoto = _photoService.SavePhoto(mentorApplication.ProfilePhoto, "Photo"),
                        cv = _photoService.SavePhoto(mentorApplication.cv, "CV"),
                        Certificates = _photoService.SavePhoto(mentorApplication.Certificates, "Certificates")

                    };




                   await _actorRepository.AddActor(newMentor);
                    await _userManager.AddToRoleAsync(CurrentUser, "Mentor");
                    TempData["isApplied"] = true;

                }
                else
                {


                  var  CurrMentor =await _actorRepository.GetActorById(CurrentUser.Mentor.ActorId);

                    //CurrMentor.mentorId = Guid.NewGuid().ToString(),
                    CurrMentor.ActorName = mentorApplication.ActorName;
                    CurrMentor.About = mentorApplication.About;
                    CurrMentor.Address = mentorApplication.Address;
                    CurrMentor.DateOfBirth = mentorApplication.DateOfBirth;
                    CurrMentor.Skills = mentorApplication.Skills;
                    CurrMentor.AcademicQualification = mentorApplication.AcademicQualification;
                    CurrMentor.CurrentProfession = mentorApplication.CurrentProfession;
                    CurrMentor.Gender = mentorApplication.Gender;
                    CurrMentor.Email = mentorApplication.Email;
                    CurrMentor.Phone = mentorApplication.Phone;
                    CurrMentor.isSubscribed = true;
                    CurrMentor.isMentor = false;




                    CurrMentor.ProfilePhoto = _photoService.SavePhoto(mentorApplication.ProfilePhoto, "Photo");
                    CurrMentor.cv = _photoService.SavePhoto(mentorApplication.cv, "CV");
                    CurrMentor.Certificates = _photoService.SavePhoto(mentorApplication.Certificates, "Certificates");

                   await _actorRepository.UpdateActor(CurrMentor);

                    if (!(await _userManager.IsInRoleAsync(CurrentUser, "Mentor")))
                    {
                        await _userManager.AddToRoleAsync(CurrentUser, "Mentor");
                    }
                    TempData["isApplied"] = true;

                }




            }
            
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ApproveActor(string actorId)
        {
            var actor = await _actorRepository.GetActorById(actorId);

          

            if (actor == null)
            {
                return NotFound();
            }

            actor.isMentor = true;
            actor.isMantee = true;

            await _actorRepository.UpdateActor(actor);
            TempData["isApproved"] = true;
            // Redirect back to the ActorsList action after approval

            var url = Url.Action("ViewProfile", "Mentor", new { actorId = actorId }, Request.Scheme);

            Response.OnCompleted(async () =>
            {
                await _ActionMessageSender.SendActionMessage(actor.UserId, url);
            });
            return RedirectToAction(nameof(ViewProfile),new {actorId = actorId });
        }


        //[HttpPost]
        //public async Task<IActionResult> LovePost(string ActorId)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Implement a method to get the current logged-in user's Id
        //    var post = await _actorRepository.GetActorById(ActorId);
        //    var isLoved = post.Loved.Select(l => l.UserNameIdentifier == userId).FirstOrDefault();
        //    if (post != null)
        //    {
        //        if (isLoved)
        //        {
        //            var curLoved = post.Loved.Select(l => l).Where(l => l.UserNameIdentifier == userId).FirstOrDefault();
        //            post.Loved.Remove(curLoved); // Remove the user's Id from the Loved list
        //            post.Votes--; // Decrease the total number of votes/loves for the post
        //        }
        //        else
        //        {
        //            post.Loved.Add(new ActorLoved { UserNameIdentifier = userId }); // Add the user's Id to the Loved list
        //            post.Votes++; // Increment the total number of votes/loves for the post
        //        }

        //        await _actorRepository.UpdateActor(post);
        //    }

        //    return RedirectToAction("ViewProfile", new { actorId = ActorId });
        //}

        [HttpPost]
        public async Task<IActionResult> LovePost(string ActorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Implement a method to get the current logged-in user's Id
            var post = await _actorRepository.GetActorById(ActorId);

            if (post != null)
            {
                var isLoved = post.Loved.Any(l => l.UserNameIdentifier == userId);

                if (isLoved)
                {
                    var curLoved = post.Loved.FirstOrDefault(l => l.UserNameIdentifier == userId);
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





        public async Task<IActionResult> Filter(string searchTerm="", int page = 1)
        {
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


            return View(new FilterAndPaginationModel {paginatedList = paginatedList, mentorFilter =new MentorFilter() });
        }






        //[HttpPost]
        //public IActionResult Filter(MentorFilter mentorFilter, int page = 1)
        //{
        //    const int pageSize = 10; // Number of items per page

        //    IQueryable<Actor> filteredActors = appDbContext.Actors; // Assuming you have an Actor DbSet in ApplicationDbContext

        //    // Apply filtering based on the form inputs
        //    if (!string.IsNullOrEmpty(mentorFilter.Name))
        //        filteredActors = filteredActors.Where(a => a.ActorName.ToLower().Contains(mentorFilter.Name.ToLower()));

        //    //if (!string.IsNullOrEmpty(mentorFilter.Skills))
        //    //    filteredActors = filteredActors.Where(a => a.Skills.Contains(mentorFilter.Skills));

        //    //if (!string.IsNullOrEmpty(mentorFilter.Gender))
        //    //    filteredActors = filteredActors.Where(a => a.Gender == mentorFilter.Gender);


        //    //if (!string.IsNullOrEmpty(mentorFilter.Gender))
        //    //    filteredActors = filteredActors.Where(a => a.Gender == mentorFilter.Gender);


        //    //if (!string.IsNullOrEmpty(mentorFilter.CurrentProfession))
        //    //    filteredActors = filteredActors.Where(a => a.CurrentProfession == mentorFilter.CurrentProfession);


        //    //if (!string.IsNullOrEmpty(mentorFilter.AcademicQualification))
        //    //    filteredActors = filteredActors.Where(a => a.AcademicQualification == mentorFilter.AcademicQualification);
        //    if (!string.IsNullOrEmpty(mentorFilter.Skills))
        //        filteredActors = filteredActors.Where(a => a.Skills.ToLower().Contains(mentorFilter.Skills.ToLower()));

        //    if (!string.IsNullOrEmpty(mentorFilter.Gender))
        //        filteredActors = filteredActors.Where(a => a.Gender.ToLower() == mentorFilter.Gender.ToLower());

        //    if (!string.IsNullOrEmpty(mentorFilter.CurrentProfession))
        //        filteredActors = filteredActors.Where(a => a.CurrentProfession.ToLower() == mentorFilter.CurrentProfession.ToLower());

        //    if (!string.IsNullOrEmpty(mentorFilter.AcademicQualification))
        //        filteredActors = filteredActors.Where(a => a.AcademicQualification.ToLower() == mentorFilter.AcademicQualification.ToLower());

        //    if (mentorFilter.age !=null && mentorFilter.EndAge != null && mentorFilter.EndAge !=0)
        //    {

        //        DateTime currentDate = DateTime.Now;
        //        DateTime maxAgeDateOfBirth = currentDate.AddYears(-mentorFilter.age);
        //        DateTime minAgeDateOfBirth = currentDate.AddYears(-mentorFilter.EndAge);


        //        filteredActors = filteredActors.Where(a => a.DateOfBirth >= minAgeDateOfBirth && a.DateOfBirth <= maxAgeDateOfBirth);

        //    }



        //    int totalItems = filteredActors.Count();
        //    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //    // Apply pagination using PaginatedList<T>
        //    List<Actor> pagedActors = filteredActors.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //   // pagedActors = pagedActors.Where(a => a.age >= mentorFilter.age && a.age <= mentorFilter.age).ToList();
        //    var paginatedList = new PaginatedList<Actor>(pagedActors, page, pageSize, totalItems, totalPages);

        //    return View(new FilterAndPaginationModel { paginatedList= paginatedList, mentorFilter=mentorFilter});
        //}
        [HttpPost]
        public IActionResult Filter(MentorFilter mentorFilter, int page = 1)
        {
            const int pageSize = 10; // Number of items per page

            IQueryable<Actor> filteredActors = appDbContext.Actors; // Assuming you have an Actor DbSet in ApplicationDbContext

            // Apply filtering based on the form inputs
            if (!string.IsNullOrEmpty(mentorFilter.Name))
                filteredActors = filteredActors.Where(a => a.ActorName.ToLower().Contains(mentorFilter.Name.ToLower()));

            if (!string.IsNullOrEmpty(mentorFilter.Skills))
                filteredActors = filteredActors.Where(a => a.Skills.ToLower().Contains(mentorFilter.Skills.ToLower()));

            if (!string.IsNullOrEmpty(mentorFilter.Gender))
                filteredActors = filteredActors.Where(a => a.Gender.ToLower().Contains(mentorFilter.Gender.ToLower()));

            if (!string.IsNullOrEmpty(mentorFilter.CurrentProfession))
                filteredActors = filteredActors.Where(a => a.CurrentProfession.ToLower().Contains(mentorFilter.CurrentProfession.ToLower()));

            if (!string.IsNullOrEmpty(mentorFilter.AcademicQualification))
                filteredActors = filteredActors.Where(a => a.AcademicQualification.ToLower().Contains(mentorFilter.AcademicQualification.ToLower()));

            if (mentorFilter.age != null && mentorFilter.EndAge != null && mentorFilter.EndAge != 0)
            {
                DateTime currentDate = DateTime.Now;
                DateTime maxAgeDateOfBirth = currentDate.AddYears(-mentorFilter.age);
                DateTime minAgeDateOfBirth = currentDate.AddYears(-mentorFilter.EndAge);
           
                filteredActors = filteredActors.Where(a => a.DateOfBirth >= minAgeDateOfBirth && a.DateOfBirth <= maxAgeDateOfBirth);
            }

            int totalItems = filteredActors.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Apply pagination using PaginatedList<T>
            List<Actor> pagedActors = filteredActors.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var paginatedList = new PaginatedList<Actor>(pagedActors, page, pageSize, totalItems, totalPages);

            return View(new FilterAndPaginationModel { paginatedList = paginatedList, mentorFilter = mentorFilter });
        }






        [HttpGet]
        public IActionResult Info(string ActorId)
        {
            return View();

        }

        public async Task<IActionResult> ViewYourProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.Users
           .Include(u => u.Mentor) // Include the mentor property
           .FirstOrDefaultAsync(u => u.Id == userId);

            return RedirectToAction("ViewProfile",new { actorId = CurrentUser.Mentor.ActorId });
        }

        public async Task<IActionResult> PostPone(string mentorId)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var mentor = await _actorRepository.GetActorById(mentorId);

            mentor.isMentor = false;

            await _actorRepository.UpdateActor(mentor);

            return RedirectToAction("ViewProfile", new {actorId = mentorId});
        }




        public async Task<IActionResult> EnableMentoring(string mentorId)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var mentor = await _actorRepository.GetActorById(mentorId);

            mentor.isMentor = true;

            await _actorRepository.UpdateActor(mentor);
            return RedirectToAction("ViewProfile", new { actorId = mentorId });
        }

       



       [HttpPost]
        public async Task<IActionResult> DeleteMentor(string actorId)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            try
            {
                var mentor = await _actorRepository.GetActorById(actorId);

                if (mentor == null)
                {
                    return NotFound();
                }

                var userId = mentor.UserId;
                var user = await _userManager.FindByIdAsync(userId);
               await _userManager.DeleteAsync(user);
                await _actorRepository.DeleteActor(mentor);
        

                return RedirectToAction("Index","Mentor");
            }
            catch (Exception ex)
            {
              
                return View("ErrorMentor");
            }
        }




    }
}
