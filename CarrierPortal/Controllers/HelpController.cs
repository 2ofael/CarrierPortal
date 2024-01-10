using Microsoft.AspNetCore.Mvc;

namespace CarrierPortal.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
