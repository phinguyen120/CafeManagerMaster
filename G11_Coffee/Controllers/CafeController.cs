using G11_Coffee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;

public class CafeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public CafeController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    // GET: Cafe
    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimUserLogin.Id));

            // Fetch cafes associated with the user
            var cafes = await _context.UserCafes
                .Where(uc => uc.UserId == UserId)
                .Select(uc => new CafeViewModel
                {
                    Id = uc.Cafe.Id,
                    Name = uc.Cafe.Name,
                    Address = uc.Cafe.Address,
                    Phone = uc.Cafe.Phone,
                    Image = uc.Cafe.Image
                })
                .ToListAsync();

            return View(cafes);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }
    }


    // GET: Cafe/CreatePartial
    public IActionResult CreatePartial()
    {
        return PartialView("_CreatePartial", new CafeViewModel());
    }

    // POST: Cafe/CreatePartial
    [HttpPost]
    public async Task<IActionResult> CreatePartial(CafeViewModel model)
    {
        try
        {
            ModelState.Remove("Image");
            var UserId = User.FindFirstValue(ClaimUserLogin.Id);

            string imagePath = null;

            // Kiểm tra và lưu hình ảnh nếu có
            if (model.ImageFile != null)
            {
                imagePath = "/images/" + await SaveImage(model.ImageFile);
            }

            if (ModelState.IsValid)
            {
                var cafe = new Cafe
                {
                    Name = model.Name,
                    Address = model.Address,
                    Phone = model.Phone,
                    Image = imagePath,
                };
                _context.Cafes.Add(cafe);
                await _context.SaveChangesAsync();

                // Tạo liên kết giữa User và Cafe
                var userCafe = new UserCafe
                {
                    UserId = int.Parse(UserId),
                    CafeId = cafe.Id
                };
                _context.UserCafes.Add(userCafe);

                //var categories = new List<Category>
                //{
                //    new Category { Name = "Coffee", CafeId = cafe.Id },
                //    new Category { Name = "Tea", CafeId = cafe.Id },
                //    new Category { Name = "Pastry", CafeId = cafe.Id }
                //};
                //_context.Categories.AddRange(categories);

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cafe and categories created successfully." });
            }

            // Nếu ModelState không hợp lệ
            return Json(new { success = false, message = "Invalid data provided.", errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
        }
        catch (Exception ex)
        {
            // Log lỗi (nếu cần)
            Console.WriteLine(ex.Message);

            // Trả về lỗi dưới dạng JSON
            return Json(new { success = false, message = "An error occurred while creating the cafe.", error = ex.Message });
        }
    }


    // GET: Cafe/EditPartial/5
    public async Task<IActionResult> EditPartial(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cafe = await _context.Cafes.FindAsync(id);
        if (cafe == null)
        {
            return NotFound();
        }

        var model = new CafeViewModel
        {
            Id = cafe.Id,
            Name = cafe.Name,
            Address = cafe.Address,
            Phone = cafe.Phone,
            Image = cafe.Image
        };

        return PartialView("_EditPartial", model);
    }

    // POST: Cafe/EditPartial
    [HttpPost]
    public async Task<IActionResult> EditPartial(CafeViewModel model)
    {
        ModelState.Remove("Image");

        if (ModelState.IsValid)
        {
            var cafe = await _context.Cafes.FindAsync(model.Id);
            if (cafe == null)
            {
                return NotFound();
            }

            cafe.Name = model.Name;
            cafe.Address = model.Address;
            cafe.Phone = model.Phone;

            if (model.ImageFile != null)
            {
                var img = cafe.Image = await SaveImage(model.ImageFile);
                cafe.Image = "/images/" + img;
            }

            _context.Update(cafe);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cafe updated successfully." });
        }
        return PartialView("_EditPartial", model);
    }

    // GET: Cafe/DeletePartial/5
    public async Task<IActionResult> DeletePartial(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var cafe = await _context.Cafes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (cafe == null)
        {
            return NotFound();
        }

        var model = new CafeViewModel
        {
            Id = cafe.Id,
            Name = cafe.Name
        };

        return PartialView("_DeletePartial", model);
    }

    // POST: Cafe/DeletePartial/5
    [HttpPost, ActionName("DeletePartial")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cafe = await _context.Cafes.FindAsync(id);
        _context.Cafes.Remove(cafe);
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Cafe deleted successfully." });
    }

    private async Task<string> SaveImage(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        var fileName = imageFile.FileName;
        var filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return fileName;
    }
}
