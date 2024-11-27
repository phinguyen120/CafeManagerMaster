using G11_Coffee.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimUserLogin.Id));

            var products = _context.Products
               .Where(p => p.Cafe.UserCafes.Any(uc => uc.UserId == UserId))
               .ToList();

            return View(products);
        }
        else
        {
            return RedirectToAction("Login", "Authentication");
        }
    }

    public async Task<IActionResult> Create()
    {
        var UserId = int.Parse(User.FindFirstValue(ClaimUserLogin.Id));
        var cafes = await _context.Cafes
             .Where(c => c.UserCafes.Any(uc => uc.UserId == UserId))
             .ToListAsync();
        ViewBag.Cafes = new SelectList(cafes, "Id", "Name");
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");

        return PartialView("_ProductFormPartial", new Product());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        ModelState.Remove("Category");
        ModelState.Remove("Image");

        string imagePath = null;
        imagePath = "/images/" + await SaveImage(product.ImageFile);

        //if (ModelState.IsValid)
        //{
        if (product.ImageFile != null)
        {
            product.Image = imagePath;
        }

        _context.Add(product);
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Product created successfully" });
        //}
        // return Json(new { success = false, message = "Failed to create product" });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }
        var UserId = int.Parse(User.FindFirstValue(ClaimUserLogin.Id));
        var cafes = await _context.Cafes
               .Where(c => c.UserCafes.Any(uc => uc.UserId == UserId))
               .ToListAsync();
        ViewBag.Cafes = new SelectList(cafes, "Id", "Name");
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
        return PartialView("_ProductFormPartial", product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Product product)
    {
        ModelState.Remove("Category");
        ModelState.Remove("Image");

        string imagePath = null;
        imagePath = "/images/" + await SaveImage(product.ImageFile);

        //if (ModelState.IsValid)
        //{
        try
        {
            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            if (product.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    DeleteImage(existingProduct.Image);
                }
                product.Image = imagePath;
            }
            else
            {
                // Keep the existing image if no new image is uploaded
                product.Image = existingProduct.Image;
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Product updated successfully" });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(product.Id))
            {
                return Json(new { success = false, message = "Product not found" });
            }
            else
            {
                throw;
            }
        }
        //}
        return Json(new { success = false, message = "Failed to update product" });
    }

    [HttpGet]
    public IActionResult DeletePartial(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return PartialView("_DeletePartial", product);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePartialConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return Json(new { success = false, message = "Product not found" });
        }

        if (!string.IsNullOrEmpty(product.Image))
        {
            DeleteImage(product.Image);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return Json(new { success = true, message = "Product deleted successfully" });
    }

    public IActionResult Search(string query)
    {
        var products = string.IsNullOrWhiteSpace(query) ?
            _context.Products.Include(p => p.Category).ToList() :
            _context.Products.Include(p => p.Category)
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .ToList();
        return PartialView("_ProductGridPartial", products);
    }

    private async Task<string> SaveImage(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        var fileName = imageFile.FileName;
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return fileName;
    }

    private void DeleteImage(string imageName)
    {
        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", imageName);
        if (System.IO.File.Exists(imagePath))
        {
            System.IO.File.Delete(imagePath);
        }
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
