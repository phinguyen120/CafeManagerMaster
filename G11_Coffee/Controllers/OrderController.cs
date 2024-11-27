using G11_Coffee.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    ICompositeViewEngine _viewEngine;

    public OrderController(ApplicationDbContext context, ICompositeViewEngine viewEngine)
    {
        _context = context;
        _viewEngine = viewEngine;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var orders = await _context.Orders
            .Include(o => o.Cafe)
            .ToListAsync();
            return View(orders);
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

        // Lấy danh sách CafeId của user
        var userCafeIds = await _context.UserCafes
            .Where(uc => uc.UserId == UserId)
            .Select(uc => uc.CafeId)
            .ToListAsync();

        // Lấy danh sách sản phẩm dựa trên CafeId
        var products = await _context.Products
            .Where(p => userCafeIds.Contains(p.CafeId))
            .ToListAsync();
        ViewBag.Products = new SelectList(products, "Id", "Name");
        return PartialView("_CreateOrderPartial", new OrderViewModel { OrderDate = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var order = new Order
                {
                    CafeId = model.CafeId,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderDetails = model.OrderDetails.Select(od => new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList();

                _context.OrderDetails.AddRange(orderDetails);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating order: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while saving the order. Please try again later." });
            }
        }
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return Json(new { success = false, message = "Invalid data provided", errors });
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.Orders
      .Include(o => o.Cafe)                // Bao gồm thông tin quán Cafe
      .Include(o => o.OrderDetails)         // Bao gồm thông tin chi tiết đơn hàng
      .ThenInclude(od => od.Product)        // Bao gồm thông tin sản phẩm trong OrderDetails
      .FirstOrDefaultAsync(o => o.Id == id); // Tìm đơn hàng theo Id


        if (order == null)
        {
            return Json(new { success = false, message = "Order not found" });
        }

        return PartialView("_OrderDetailsPartial", order);
    }

    public async Task<IActionResult> Print(int id)
    {
        var order = await _context.Orders
             .Include(o => o.Cafe)
             .Include(o => o.OrderDetails)
             .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return Json(new { success = false, message = "Order not found" });
        }

        var printInvoiceView = await this.RenderViewToStringAsync("_PrintInvoicePartial", order);
        return Json(new { success = true, html = printInvoiceView });
    }

    public async Task<IActionResult> Search(string query)
    {
        var orders = await _context.Orders
             .Include(o => o.Cafe)
             .Where(o => o.OrderDate.ToString().Contains(query)
                || o.Cafe.Name.Contains(query))
             .ToListAsync();

        return PartialView("_OrderListPartial", orders);
    }

    private async Task<string> RenderViewToStringAsync(string viewName, object model)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = ControllerContext.ActionDescriptor.ActionName;
        ViewData.Model = model;

        using (var writer = new StringWriter())
        {
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewName} does not match any available view");
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}