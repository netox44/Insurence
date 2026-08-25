using Insurence.Helper;
using Insurence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Insurence.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailService _emailService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        // =========================
        // REGISTER (GET)
        // =========================
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // =========================
        // REGISTER (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email is already taken
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            // Create new IdentityUser
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Assign "User" role
            await _userManager.AddToRoleAsync(user, "User");

            // Send Welcome Email (Fail-safe, won't break registration if email fails)
            try
            {
                await _emailService.SendEmailAsync(
                    model.Email,
                    "Welcome to HM Insurance",
                    $@"<h3>Hello {model.FirstName} {model.LastName},</h3>
                       <p>Your account has been successfully created.</p>
                       <p>You can now log in to the portal.</p>"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed to send: {ex.Message}");
            }

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login", "Account");
        }

        // =========================
        // LOGIN (GET)
        // =========================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // =========================
        // LOGIN (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                userName: user.UserName!,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account is locked out. Please try again later.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        // =========================
        // LOGOUT
        // =========================
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // =========================
        // ACCESS DENIED
        // =========================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}