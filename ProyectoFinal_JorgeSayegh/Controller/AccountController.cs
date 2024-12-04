using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoFinal_JorgeSayegh.Contexts;
using ProyectoFinal_JorgeSayegh.Models;
using ProyectoFinal_JorgeSayegh.ViewModels;
using Microsoft.Extensions.Logging;

namespace ProyectoFinal_JorgeSayegh.Controller
{
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Accessing the Login page.");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login model is invalid.");
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user != null)
            {
                _logger.LogInformation($"User found: {user.Username}");

                // Validate the password hash
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    _logger.LogInformation("Password successfully validated.");

                    // Add NameIdentifier claim for proper user identification
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    _logger.LogInformation($"Login successful for user: {user.Username}");
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    _logger.LogWarning("Invalid password attempt for user: {Username}", user.Username);
                }
            }
            else
            {
                _logger.LogWarning("User not found: {Username}", model.Username);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Entering Logout method.");

            // Clear session data
            HttpContext.Session.Clear();
            _logger.LogInformation("Session cleared.");

            // Sign out user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User signed out.");

            return RedirectToAction("Login", "Account");
        }
    }
}
