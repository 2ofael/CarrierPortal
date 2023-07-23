using CarrierPortal.Models.DataModel;
using CarrierPortal.Models;
using CarrierPortal.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CarrierPortal.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly IActorRepository _actorRepository;
      

        public ApprovalController(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public IActionResult Index()
        {
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

            return Ok(); // Return an HTTP 200 response
        }



    }
}
