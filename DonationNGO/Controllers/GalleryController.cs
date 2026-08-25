using Microsoft.AspNetCore.Mvc;

namespace Insurence.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult details()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            // For demo purposes, we'll just return the view
            // In a real application, you'd fetch the image details by ID
            ViewBag.ImageId = id;
            return View();
        }
    }
}