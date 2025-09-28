using Microsoft.AspNetCore.Mvc;

namespace TravelBuddyAPI.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
