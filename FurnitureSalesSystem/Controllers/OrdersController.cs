using FurnitureSalesSystem.Data;
using FurnitureSalesSystem.Models;
using FurnitureSalesSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace FurnitureSalesSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders/Create
        [Authorize(Roles = "Nhân viên bán hàng")]
        public async Task<IActionResult> Create()
        {
            var customers = await _context.Customers.ToListAsync();
            var products = await _context.Products
                                         .Where(p => p.Status && p.Quantity > 0)
                                         .ToListAsync();

            ViewBag.CustomerList = new SelectList(customers, "Id", "FullName");
            ViewBag.Products = products;
            ViewBag.Customers = customers;

            var vm = new OrderCreateViewModel
            {
                Customers = customers,
                Products = products,
                OrderDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "Không xác định"
            };

            return View(vm);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên bán hàng")]
        public async Task<IActionResult> Create(OrderCreateViewModel vm)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || !_context.Users.Any(u => u.Id == userId))
            {
                ModelState.AddModelError("", "Tài khoản hiện tại không hợp lệ hoặc đã bị xóa.");
                vm.Customers = await _context.Customers.ToListAsync();
                vm.Products = await _context.Products
                                            .Where(p => p.Status && p.Quantity > 0)
                                            .ToListAsync();
                return View(vm);
            }

            if (!ModelState.IsValid || vm.Items == null || !vm.Items.Any())
            {
                vm.Customers = await _context.Customers.ToListAsync();
                vm.Products = await _context.Products
                                            .Where(p => p.Status && p.Quantity > 0)
                                            .ToListAsync();

                ModelState.AddModelError("", "Vui lòng chọn ít nhất một sản phẩm.");
                
                return View(vm);
            }

            var order = new Order
            {
                CustomerId = vm.CustomerId,
                OrderDate = DateTime.Now,
                UserId = userId,
                Status = vm.Status,
                InternalNote = vm.InternalNote,
                CustomerNote = vm.CustomerNote,
                TotalAmount = vm.Items.Sum(p => p.Quantity * p.UnitPrice),
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in vm.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm '{product?.Name}' không đủ số lượng.");

                    return View(vm);
                }

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });

                product.Quantity -= item.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Orders");
        }

        [Authorize(Roles = "Giám đốc, Nhân viên bán hàng")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var allOrders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var pagedOrders = allOrders.ToPagedList(pageNumber, pageSize);
            return View(pagedOrders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên bán hàng")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Xoá từng chi tiết đơn hàng trước (nếu có)
            if (order.OrderDetails != null)
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails);
            }

            // Xoá đơn hàng chính
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Giám đốc, Nhân viên bán hàng")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        [Authorize(Roles = "Nhân viên bán hàng")]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer) // ✅ Load luôn thông tin khách hàng
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderCreateViewModel
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.FullName,
                CustomerPhone = order.Customer.Phone,
                CustomerAddress = order.Customer.Address,
                Status = order.Status,
                Items = order.OrderDetails.Select(od => new OrderItemViewModel
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList(),
                InternalNote = order.InternalNote,
                CustomerNote = order.CustomerNote,
                Products = _context.Products.ToList()
            };

            ViewBag.CustomerList = new SelectList(_context.Customers, "Id", "FullName");
            ViewBag.Products = _context.Products.ToList();

            return View("Create", viewModel); // Dùng chung form Create
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên bán hàng")]
        public async Task<IActionResult> Edit(int id, OrderCreateViewModel vm)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            if (!ModelState.IsValid || vm.Items == null || !vm.Items.Any())
            {
                var customers = await _context.Customers.ToListAsync();
                vm.Customers = customers;

                vm.Products = await _context.Products
                    .Where(p => p.Status && p.Quantity > 0)
                    .ToListAsync();

                ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", vm.CustomerId);

                ModelState.AddModelError("", "Vui lòng chọn ít nhất một sản phẩm.");
                return View("Create", vm);
            }

            // 1. Hoàn trả lại số lượng sản phẩm cũ
            foreach (var od in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(od.ProductId);
                if (product != null)
                {
                    product.Quantity += od.Quantity;
                }
            }

            // 2. Xoá chi tiết cũ
            _context.OrderDetails.RemoveRange(order.OrderDetails);
            await _context.SaveChangesAsync();

            // 3. Gán lại thông tin mới
            order.CustomerId = vm.CustomerId;
            order.Status = vm.Status;
            order.TotalAmount = vm.Items.Sum(i => i.Quantity * i.UnitPrice);

            // 🔥 THÊM DÒNG NÀY để fix lỗi không lưu được ghi chú
            order.InternalNote = vm.InternalNote;
            order.CustomerNote = vm.CustomerNote;

            order.OrderDetails = new List<OrderDetail>();

            // 4. Kiểm tra tồn kho và tạo mới order detail
            foreach (var item in vm.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm '{product?.Name}' không đủ số lượng.");
                    return View("Create", vm);

                }

                product.Quantity -= item.Quantity;

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Giám đốc, Nhân viên bán hàng")]
        public async Task<IActionResult> LoadProductsPartial(int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status && p.Quantity > 0)
                .ToPagedListAsync(pageNumber, pageSize);

            return PartialView("_ProductListPartial", products); // OK
        }
    }
}
