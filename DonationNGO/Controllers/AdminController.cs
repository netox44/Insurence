using Insurence.Models;
using Insurence.Data;
using Insurence.Helper;
using Insurence.Models.ViewModel;
using Insurence.Models.ViewModels;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DonationNGO.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // ================================================================
        // ============================ DASHBOARD ==========================
        // ================================================================
        public IActionResult Dashboard()
        {


            // Total donation amount
            ViewBag.TotalDonations = _context.Donations
                .Where(d => d.Status == "Completed")
                .Sum(d => (decimal?)d.Amount) ?? 0;

            // Total donor count (unique donors)
            ViewBag.DonationCount = _context.Donations
                .Select(d => d.UserId)
                .Distinct()
                .Count();

            // Active Programs
            ViewBag.ActivePrograms = _context.Programs
                .Where(p => p.Status == "Active")
                .Count();

            // Total Partner NGOs
            ViewBag.TotalNGOs = _context.Ngos.Count();


            // 📌 RECENT DONATIONS
            // ----------------------------
            ViewBag.RecentDonations = _context.Donations
                .OrderByDescending(d => d.Date)
                .Take(10)
                .ToList();

            return View();
        }

        // ================================================================
        // ============================ NGOS ===============================
        // ================================================================
        public IActionResult ManageNGOs(string searchQuery)
        {
            var ngos = _context.Ngos.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                ngos = ngos.Where(n =>
                    (n.Name ?? "").Contains(searchQuery) ||
                    (n.Email ?? "").Contains(searchQuery) ||
                    (n.FocusArea ?? "").Contains(searchQuery)
                );
            }

            ViewBag.SearchQuery = searchQuery;
            return View(ngos.AsNoTracking().ToList());
        }

        public IActionResult CreateNGO() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNGO(Ngo ngo)
        {
            if (!ModelState.IsValid)
                return View(ngo);

            ngo.PartnershipDate = DateTime.Now;
            _context.Ngos.Add(ngo);
            _context.SaveChanges();

            TempData["Success"] = "NGO added successfully!";
            return RedirectToAction(nameof(ManageNGOs));
        }

        public IActionResult EditNGO(int id)
        {
            var ngo = _context.Ngos.Find(id);
            if (ngo == null) return NotFound();
            return View(ngo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNGO(Ngo ngo)
        {
            if (!ModelState.IsValid) return View(ngo);

            var dbNgo = _context.Ngos.Find(ngo.Id);
            if (dbNgo == null) return NotFound();

            dbNgo.Name = ngo.Name;
            dbNgo.FocusArea = ngo.FocusArea;
            dbNgo.Description = ngo.Description;
            dbNgo.Email = ngo.Email;
            dbNgo.Phone = ngo.Phone;
            dbNgo.Website = ngo.Website;
            dbNgo.Status = ngo.Status;

            _context.Update(dbNgo);
            _context.SaveChanges();

            TempData["Success"] = "NGO updated successfully!";
            return RedirectToAction(nameof(ManageNGOs));
        }

        public IActionResult DeleteNGO(int id)
        {
            var ngo = _context.Ngos.Find(id);
            if (ngo == null) return NotFound();
            return View(ngo);
        }

        [HttpPost, ActionName("DeleteNGO")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNGOConfirmed(int id)
        {
            var ngo = _context.Ngos.Find(id);
            if (ngo == null) return NotFound();

            _context.Ngos.Remove(ngo);
            _context.SaveChanges();

            TempData["Success"] = "NGO deleted successfully!";
            return RedirectToAction(nameof(ManageNGOs));
        }

        [AllowAnonymous]
        public IActionResult DetailsNGO(int id)
        {
            var ngo = _context.Ngos.Find(id);
            if (ngo == null) return NotFound();
            return View(ngo);
        }

        // ================================================================
        // ============================ DONATIONS ==========================
        // ================================================================
        [Authorize(Roles = "Admin")]
        public IActionResult ManageDonations(string searchTerm, string status, string program, string dateRange)
        {
            var query = _context.Donations.AsQueryable();

            // ----------------------------
            // 📌 FILTERING
            // ----------------------------

            // Search (DonorName, Program, PaymentMethod)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d =>
                    d.DonorName.Contains(searchTerm) ||
                    d.Program.Contains(searchTerm) ||
                    d.PaymentMethod.Contains(searchTerm)
                );
            }

            // Status
            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            // Program
            if (!string.IsNullOrEmpty(program))
                query = query.Where(d => d.Program == program);

            // Date Range
            if (dateRange == "today")
                query = query.Where(d => d.Date.Date == DateTime.Today);
            else if (dateRange == "week")
                query = query.Where(d => d.Date >= DateTime.Today.AddDays(-7));
            else if (dateRange == "month")
                query = query.Where(d => d.Date >= DateTime.Today.AddMonths(-1));
            else if (dateRange == "year")
                query = query.Where(d => d.Date >= DateTime.Today.AddYears(-1));

            var vm = new DonationFilterViewModel
            {
                SearchTerm = searchTerm,
                Status = status,
                Program = program,
                DateRange = dateRange,
                Donations = query.OrderByDescending(d => d.Date).ToList(),
                StatusList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "All", Value = "" },
                    new SelectListItem { Text = "Completed", Value = "Completed" },
                    new SelectListItem { Text = "Pending", Value = "Pending" },
                    new SelectListItem { Text = "Failed", Value = "Failed" }
                },
                ProgramList = _context.Programs
                    .Select(p => new SelectListItem
                    {
                        Text = p.ProgramName,
                        Value = p.ProgramName
                    })
                    .ToList(),
                DateRangeList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "All", Value = "" },
                    new SelectListItem { Text = "Today", Value = "today" },
                    new SelectListItem { Text = "This Week", Value = "week" },
                    new SelectListItem { Text = "This Month", Value = "month" },
                    new SelectListItem { Text = "This Year", Value = "year" }
                }
            };

            return View(vm);
        }

        public IActionResult AddDonation()
        {
            ViewBag.Programs = _context.Programs.Select(p => p.ProgramName).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDonation(Donation donation)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Programs = _context.Programs.Select(p => p.ProgramName).ToList();
                return View(donation);
            }

            donation.Date = DateTime.Now;
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Donation added successfully!";
            return RedirectToAction(nameof(ManageDonations));
        }

        public async Task<IActionResult> DonationDetails(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();
            return View(donation);
        }

        // ================================================================
        // ============================ PROGRAMS ===========================
        // ================================================================
        public IActionResult ManagePrograms(string search, string category, int page = 1)
        {
            int pageSize = 6; // Reduced to match card layout better

            var programs = _context.Programs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                programs = programs.Where(p => p.ProgramName.Contains(search) || p.Description.Contains(search));

            if (!string.IsNullOrEmpty(category) && category != "All")
                programs = programs.Where(p => p.Category == category);

            int total = programs.Count();

            var vm = new Insurence.Models.ViewModel.ProgramFilterViewModel
            {
                Search = search,
                Category = category,
                Programs = programs
                            .OrderByDescending(p => p.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToList(),
                Page = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };

            return View(vm);
        }

        // Remove the CreateProgram GET action since we're using modal
        // public IActionResult CreateProgram() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgram(string ProgramName, string Category, string Description,
            decimal FundingGoal, int DurationMonths, IFormFile programImage)
        {
            try
            {
                // Create new program object from form data
                var program = new prmodel
                {
                    ProgramName = ProgramName,
                    Category = Category,
                    Description = Description,
                    FundingGoal = FundingGoal,
                    DurationMonths = DurationMonths,
                    AmountRaised = 0,
                    Status = "Active",
                    CreatedAt = DateTime.Now
                };

                // Handle image upload
                if (programImage != null && programImage.Length > 0)
                {
                    program.ImageUrl = FileHelper.SaveFile(programImage, "images/programs");
                }
                else
                {
                    // Set default image or handle as needed
                    program.ImageUrl = "images/programs/default-program.jpg";
                }

                _context.Programs.Add(program);
                await _context.SaveChangesAsync();

                TempData["Success"] = "policy created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating program: {ex.Message}";
            }

            return RedirectToAction(nameof(ManagePrograms));
        }

        public IActionResult EditProgram(int id)
        {
            var program = _context.Programs.Find(id);
            if (program == null) return NotFound();
            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProgram(int id, prmodel program, IFormFile programImage)
        {
            if (id != program.Id) return NotFound();

            if (!ModelState.IsValid) return View(program);

            var dbProgram = _context.Programs.Find(id);
            if (dbProgram == null) return NotFound();

            dbProgram.ProgramName = program.ProgramName;
            dbProgram.Category = program.Category;
            dbProgram.Description = program.Description;
            dbProgram.FundingGoal = program.FundingGoal;
            dbProgram.DurationMonths = program.DurationMonths;
            dbProgram.Status = program.Status;

            if (programImage != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(dbProgram.ImageUrl))
                    FileHelper.DeleteFile(dbProgram.ImageUrl);

                dbProgram.ImageUrl = FileHelper.SaveFile(programImage, "images/programs");
            }

            _context.Update(dbProgram);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Policy updated successfully!";
            return RedirectToAction(nameof(ManagePrograms));
        }

        // Remove the DeleteProgram GET action since we're using inline form
        // public IActionResult DeleteProgram(int id)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProgram(int id)
        {
            try
            {
                var program = _context.Programs.Find(id);
                if (program == null)
                {
                    TempData["Error"] = "Policy not found.";
                    return RedirectToAction(nameof(ManagePrograms));
                }

                // Delete associated image file
                if (!string.IsNullOrEmpty(program.ImageUrl))
                    FileHelper.DeleteFile(program.ImageUrl);

                _context.Programs.Remove(program);
                _context.SaveChanges();

                TempData["Success"] = "Policy deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting program: {ex.Message}";
            }

            return RedirectToAction(nameof(ManagePrograms));
        }

        public IActionResult ProgramDetails(int id)
        {
            var program = _context.Programs.Find(id);
            if (program == null) return NotFound();
            return View(program);
        }

        // ================================================================
        // ============================ GALLERY ============================
        // ================================================================
        [Authorize(Roles = "Admin")]
        public IActionResult ManageGallery()
        {
            var images = _context.Galleries
                .OrderByDescending(g => g.UploadDate)
                .ToList();

            return View(images);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadGallery(List<IFormFile> imageFiles, string category, string description)
        {
            try
            {
                if (imageFiles == null || imageFiles.Count == 0)
                {
                    TempData["Error"] = "Please select at least one image.";
                    return RedirectToAction("ManageGallery");
                }

                foreach (var file in imageFiles)
                {
                    // Validate file
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                    if (!allowed.Contains(extension))
                    {
                        TempData["Error"] = "Only image formats JPG, PNG, GIF, WEBP are allowed.";
                        return RedirectToAction("ManageGallery");
                    }

                    // Create upload folder if not exists
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "gallery");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    // Generate unique file name
                    var uniqueName = Guid.NewGuid().ToString() + extension;
                    var fullPath = Path.Combine(uploadPath, uniqueName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Save record to database
                    var galleryItem = new Gallery
                    {
                        ImageUrl = $"uploads/gallery/{uniqueName}",
                        Category = category,
                        Description = description,
                        UploadDate = DateTime.Now
                    };

                    _context.Galleries.Add(galleryItem);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Images uploaded successfully!";
            }
            catch
            {
                TempData["Error"] = "Something went wrong. Try again.";
            }

            return RedirectToAction("ManageGallery");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteGallery(int id)
        {
            var img = _context.Galleries.FirstOrDefault(x => x.Id == id);

            if (img == null)
            {
                TempData["Error"] = "Image not found.";
                return RedirectToAction("ManageGallery");
            }

            // Delete file from server
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            // Remove DB record
            _context.Galleries.Remove(img);
            _context.SaveChanges();

            TempData["Success"] = "Image deleted successfully.";
            return RedirectToAction("ManageGallery");
        }

        // ================================================================
        // ============================ USERS =============================
        // ================================================================
        public async Task<IActionResult> ManageUsers(string search, string role)
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var totalDonations = await _context.Donations
                    .Where(d => d.UserId == user.Id)
                    .SumAsync(d => (decimal?)d.Amount) ?? 0;

                model.Add(new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    JoinDate = DateTime.Now,
                    IsActive = !await _userManager.IsLockedOutAsync(user),
                    TotalDonations = totalDonations
                });
            }

            if (!string.IsNullOrEmpty(search))
                model = model.Where(u =>
                    (u.FullName ?? "").Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email ?? "").Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!string.IsNullOrEmpty(role) && role != "All Roles")
                model = model.Where(u => u.Roles.Contains(role)).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.SetLockoutEndDateAsync(user, null);

            return RedirectToAction(nameof(ManageUsers));
        }

        // ================================================================
        // ============================ REPORTS ===========================
        // ================================================================
        public IActionResult Reports()
        {
            var model = new Insurence.Models.ViewModel.ReportViewModel();

            // ------------------- Metrics -------------------
            model.TotalDonations = _context.Donations
                .Where(d => d.Status == "Completed")
                .Sum(d => (decimal?)d.Amount) ?? 0;

            var completedDonations = _context.Donations
                .Where(d => d.Status == "Completed");

            model.AverageDonation = completedDonations.Any()
                ? completedDonations.Average(d => d.Amount)
                : 0;

            model.NewDonors = _context.Donations
                .Where(d => d.Date >= DateTime.Today.AddDays(-30))
                .Select(d => d.UserId)
                .Distinct()
                .Count();

            var totalDonations = _context.Donations.Count();
            model.SuccessRate = totalDonations > 0
                ? (_context.Donations.Count(d => d.Status == "Completed") * 100.0 / totalDonations)
                : 0;

            // ------------------- Program Performance -------------------
            model.ProgramPerformances = _context.Programs
                .Select(p => new ProgramPerformance
                {
                    ProgramName = p.ProgramName,
                    FundsRaised = p.AmountRaised,
                    Goal = p.FundingGoal,
                    Donors = _context.Donations.Count(d => d.Program == p.ProgramName && d.Status == "Completed"),
                    Status = p.Status
                })
                .ToList();

            // ------------------- Donation Trends (last 7 days) -------------------
            model.DonationTrends = _context.Donations
                .Where(d => d.Status == "Completed" && d.Date >= DateTime.Today.AddDays(-7))
                .GroupBy(d => d.Date.Date)
                .Select(g => new DonationTrend
                {
                    Date = g.Key,
                    Amount = g.Sum(d => d.Amount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            return View(model);
        }

        // ================================================================
        // ============================ EXPORT PDF =========================
        // ================================================================
        public IActionResult ExportDonationsPdf(string searchTerm, string status, string program, string dateRange)
        {
            var donationsQuery = _context.Donations.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                donationsQuery = donationsQuery
                    .Where(d => d.DonorName.Contains(searchTerm) || d.Program.Contains(searchTerm));

            if (!string.IsNullOrEmpty(status) && status != "All Status")
                donationsQuery = donationsQuery.Where(d => d.Status == status);

            if (!string.IsNullOrEmpty(program) && program != "All Programs")
                donationsQuery = donationsQuery.Where(d => d.Program == program);

            if (!string.IsNullOrEmpty(dateRange))
            {
                var today = DateTime.Today;
                donationsQuery = dateRange switch
                {
                    "Last 7 days" => donationsQuery.Where(d => d.Date >= today.AddDays(-7)),
                    "Last 30 days" => donationsQuery.Where(d => d.Date >= today.AddDays(-30)),
                    "Last 90 days" => donationsQuery.Where(d => d.Date >= today.AddDays(-90)),
                    _ => donationsQuery
                };
            }

            var donations = donationsQuery.OrderByDescending(d => d.Date).ToList();

            using var stream = new MemoryStream();
            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20, 20, 20, 20);
            PdfWriter.GetInstance(doc, stream);

            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Insurance REPORT"));
            doc.Add(new iTextSharp.text.Paragraph("Generated: " + DateTime.Now));
            doc.Add(new iTextSharp.text.Paragraph("\n"));

            PdfPTable table = new PdfPTable(5);
            table.AddCell("Date");
            table.AddCell("Email");
            table.AddCell("Insurance");
            table.AddCell("Amount");
            table.AddCell("Status");

            foreach (var d in donations)
            {
                table.AddCell(d.Date.ToString("yyyy-MM-dd"));
                table.AddCell(d.DonorName);
                table.AddCell(d.Program);
                table.AddCell(d.Amount.ToString("C"));
                table.AddCell(d.Status);
            }

            doc.Add(table);
            doc.Close();

            return File(stream.ToArray(), "application/pdf", "InsuranceRecord.pdf");
        }
    }
}