using Microsoft.AspNetCore.Mvc;

namespace CarrierPortal.Controllers
{
    public class MentorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
