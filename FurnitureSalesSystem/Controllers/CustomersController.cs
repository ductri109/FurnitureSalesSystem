using FurnitureSalesSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FurnitureSalesSystem.Controllers
{
    [Authorize(Roles = "Giám đốc, Nhân viên bán hàng")]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCustomers / PageSize);

            return View(customers);
        }
    }
}
