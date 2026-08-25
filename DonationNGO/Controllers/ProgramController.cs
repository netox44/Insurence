using Microsoft.AspNetCore.Mvc;

namespace Insurence.Controllers
{
    public class ProgramController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}