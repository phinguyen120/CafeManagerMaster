using G11_Coffee.Models;
using G11_Coffee.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace G11_Coffee.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Fetch all users and their associated cafes
                var users = await _context.Users
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Cafes = string.Join(", ", u.userCafes.Select(uc => uc.Cafe.Name))  // Join cafes' names
                    })
                    .ToListAsync();

                return View(users);  // Pass the list of UserViewModel to the view
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }






    }
}
