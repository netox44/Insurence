using Microsoft.AspNetCore.Mvc;

namespace Insurence.Controllers
{
    public class DonationController : Controller
    {
        public IActionResult Donate()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
