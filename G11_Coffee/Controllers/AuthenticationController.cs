// Controllers/AuthenticationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using G11_Coffee.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AuthenticationController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthenticationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (dbUser != null && VerifyPassword(user.Password, dbUser.Password))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimUserLogin.Id, dbUser.Id.ToString()),
                new Claim(ClaimUserLogin.Email, user.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Cho phép "remember me"
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(user);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> Register(User user)
    {
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (dbUser != null)
        {
            ModelState.AddModelError(string.Empty, "The account has existed.");
            return View(user);
        }
        else
        {
            User newUser = new User
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
            };
            var register = _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Authentication");
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Authentication");
    }

    private bool VerifyPassword(string inputPassword, string storedPassword)
    {
        // Implement password verification logic here
        // For example, using BCrypt or another secure hashing method
        return inputPassword == storedPassword; // This is not secure, replace with proper verification
    }


}