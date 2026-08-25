using Insurence.Data;
using Insurence.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Insurence.Controllers
{
    [Authorize(Roles = "User")] // Only logged-in users can access these features
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ======================================================
        // =============== USER DASHBOARD =======================
        // ======================================================
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var donations = _context.Donations.Where(d => d.UserId == user.Id).ToList();

            ViewBag.TotalDonated = donations.Sum(d => d.Amount);
            ViewBag.TotalDonations = donations.Count;
            ViewBag.ActivePrograms = _context.Programs.Count();
            ViewBag.RecentDonations = donations.OrderByDescending(d => d.Date).Take(5).ToList();

            decimal totalDonated = ViewBag.TotalDonated;
            ViewBag.DonorLevel = totalDonated >= 5000 ? "Platinum" :
                                 totalDonated >= 2000 ? "Gold" :
                                 totalDonated >= 500 ? "Silver" : "Bronze";

            return View();
        }

        // ======================================================
        // =============== DONATE PAGE ==========================
        // ======================================================
        public IActionResult Donate(string cause)
        {
            ViewBag.Cause = cause;

            // Load all programs dynamically
            ViewBag.Programs = _context.Programs.OrderByDescending(p => p.Id).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(decimal amount, string cause, string paymentMethod)
        {
            if (amount <= 0 || string.IsNullOrEmpty(cause) || string.IsNullOrEmpty(paymentMethod))
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                ViewBag.Programs = _context.Programs.ToList();
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // ----------------- Save Donation -----------------
            var donation = new Donation
            {
                DonorName = user.UserName,
                Program = cause,
                Amount = amount,
                PaymentMethod = paymentMethod,
                Date = DateTime.Now,
                Status = "Completed",
                UserId = user.Id
            };

            _context.Donations.Add(donation);

            // ----------------- Update Program AmountRaised & DonorsCount -----------------
            var program = _context.Programs.FirstOrDefault(p => p.ProgramName == cause);
            if (program != null)
            {
                program.AmountRaised += amount;
                program.DonorsCount = _context.Donations.Count(d => d.Program == cause) + 1; // +1 for current donation
                _context.Programs.Update(program);
            }

            await _context.SaveChangesAsync();

            ViewBag.Message = "Your insurance has been successfully completed. Thank you for choosing us.";
            ViewBag.Programs = _context.Programs.ToList();
            return View();
        }



        // ======================================================
        // =============== MY DONATIONS =========================
        // ======================================================
        public async Task<IActionResult> MyDonations()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var donations = _context.Donations
                                    .Where(d => d.UserId == user.Id)
                                    .OrderByDescending(d => d.Date)
                                    .ToList();

            return View(donations);
        }

        // ======================================================
        // =============== PROGRAMS LIST ========================
        // ======================================================
        public IActionResult Programs()
        {
            // Load all programs
            var programs = _context.Programs
                .OrderByDescending(p => p.Id)
                .ToList();

            // Calculate AmountRaised and DonorsCount for each program
            foreach (var p in programs)
            {
                p.AmountRaised = _context.Donations
                    .Where(d => d.Program == p.ProgramName)
                    .Sum(d => (decimal?)d.Amount) ?? 0;

                p.DonorsCount = _context.Donations
                    .Count(d => d.Program == p.ProgramName);
            }

            return View(programs);
        }

        // ======================================================
        // =============== PROGRAM DETAILS ======================
        // ======================================================
        public IActionResult ProgramDetails(int id)
        {
            var program = _context.Programs.Find(id);
            if (program == null) return NotFound();
            return View(program);
        }

        // ======================================================
        // =============== GALLERY ==============================
        // ======================================================
        public IActionResult Gallery()
        {
            var images = _context.Galleries.OrderByDescending(g => g.Id).ToList();
            return View(images);
        }

        // ======================================================
        // =============== HELP & SUPPORT =======================
        // ======================================================
        [HttpGet]
        public IActionResult HelpSupport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HelpSupport(string subject, string priority, string message, IFormFile[] attachments)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                ViewBag.Error = "Please fill all required fields.";
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var query = new SupportQuery
            {
                UserId = user.Id,
                Subject = subject,
                Priority = string.IsNullOrEmpty(priority) ? "Medium" : priority,
                Message = message,
                SubmissionDate = DateTime.Now
            };

            if (attachments != null && attachments.Length > 0)
            {
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "support");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        query.AttachmentPath = fileName;
                    }
                }
            }

            _context.SupportQueries.Add(query);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Your query has been submitted successfully!";
            return View();
        }

        // ======================================================
        // =============== EXPORT DONATIONS PDF =================
        // ======================================================
        public async Task<IActionResult> ExportPdf()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var donations = _context.Donations
                                    .Where(d => d.UserId == user.Id)
                                    .OrderByDescending(d => d.Date)
                                    .ToList();

            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                var writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                doc.Add(new iTextSharp.text.Paragraph("My Insurance Report"));
                doc.Add(new iTextSharp.text.Paragraph("Generated On: " + DateTime.Now));
                doc.Add(new iTextSharp.text.Paragraph("\n"));

                PdfPTable table = new PdfPTable(4);
                table.AddCell("Date");
                table.AddCell("Insurance");
                table.AddCell("Amount");
                table.AddCell("Status");

                foreach (var d in donations)
                {
                    table.AddCell(d.Date.ToString("yyyy-MM-dd"));
                    table.AddCell(d.Program);
                    table.AddCell(d.Amount.ToString());
                    table.AddCell(d.Status);
                }

                doc.Add(table);
                doc.Close();

                return File(stream.ToArray(), "application/pdf", "My-Insurance.pdf");
            }
        }
    }
}
