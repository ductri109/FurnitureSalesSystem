using FurnitureSalesSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FurnitureSalesSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureSalesSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Giám đốc")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Tổng số sản phẩm
            var totalProducts = await _context.Products.CountAsync();

            // Tổng số đơn hàng
            var totalOrders = await _context.Orders.CountAsync();

            // Tổng doanh thu từ các đơn hàng đã xác nhận
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == OrderStatus.Confirmed)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            // Tổng số khách hàng
            var totalCustomers = await _context.Customers.CountAsync();

            // Thống kê trạng thái đơn hàng (Pending, Confirmed, Cancelled)
            var orderStatusStats = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // 5 đơn hàng mới nhất
            var latestOrders = await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            // Gửi dữ liệu sang view qua ViewBag
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.OrderStatusStats = orderStatusStats;
            ViewBag.OrderStatusLabels = orderStatusStats.Select(s => s.Status).ToList();
            ViewBag.OrderStatusCounts = orderStatusStats.Select(s => (int)s.Count).ToList();
            ViewBag.LatestOrders = latestOrders;

            return View();
        }
    }
}
