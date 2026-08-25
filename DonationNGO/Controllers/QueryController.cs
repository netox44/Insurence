using Microsoft.AspNetCore.Mvc;

namespace Insurence.Controllers
{
    public class QueryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(string name, string email, string subject, string message, string category)
        {
            // In a real application, you'd save this to a database
            // For now, we'll just show a success message

            ViewBag.Message = "Thank you for your query! We'll get back to you within 24 hours.";
            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewBag.Subject = subject;

            return View("Index");
        }

        [HttpPost]
        public IActionResult Contact(string name, string email, string phone, string subject, string message)
        {
            // Handle contact form submission
            ViewBag.Message = "Thank you for contacting us! We'll respond to your message as soon as possible.";
            ViewBag.Name = name;

            return View("Index");
        }
    }
}