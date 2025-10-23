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
                .Where(p => p.Status) // bỏ điều kiện Quantity > 0
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
                await LoadViewData(vm);
                return View(vm);
            }

            if (!ModelState.IsValid || vm.Items == null || !vm.Items.Any())
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một sản phẩm.");
                await LoadViewData(vm);
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
                TotalAmount = vm.Items.Sum(i => i.Quantity * i.UnitPrice),
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in vm.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Sản phẩm có ID {item.ProductId} không tồn tại.");
                    await LoadViewData(vm);
                    return View(vm);
                }

                if (product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm '{product.Name}' chỉ còn {product.Quantity} sản phẩm trong kho.");
                    await LoadViewData(vm);
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

            return RedirectToAction(nameof(Index));
        }

        // ✅ Hàm riêng để nạp lại dữ liệu cho View khi có lỗi
        private async Task LoadViewData(OrderCreateViewModel vm)
        {
            var customers = await _context.Customers.ToListAsync();
            var products = await _context.Products
                .Where(p => p.Status) // bỏ điều kiện Quantity > 0
                .ToListAsync();

            ViewBag.CustomerList = new SelectList(customers, "Id", "FullName", vm.CustomerId);
            ViewBag.Products = products;
            ViewBag.Customers = customers;

            vm.Customers = customers;
            vm.Products = products;
        }

        [Authorize(Roles = "Giám đốc, Nhân viên bán hàng")]
        public async Task<IActionResult> Index(string? statusFilter, string? customerName, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var ordersQuery = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            // 🔹 Lọc theo trạng thái
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<OrderStatus>(statusFilter, out var parsedStatus))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == parsedStatus);
            }

            // 🔹 Lọc theo tên khách hàng
            if (!string.IsNullOrEmpty(customerName))
            {
                ordersQuery = ordersQuery.Where(o => o.Customer != null && o.Customer.FullName.Contains(customerName));
            }

            // ✅ Dùng ToPagedListAsync để tránh cảnh báo
            var pagedOrders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToPagedListAsync(pageNumber, pageSize);

            ViewBag.StatusList = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = (statusFilter == s.ToString())
                }).ToList();

            ViewBag.CurrentStatus = statusFilter;
            ViewBag.CurrentCustomer = customerName;

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
                if (product == null)
                {
                    ModelState.AddModelError("", $"Sản phẩm có ID {item.ProductId} không tồn tại.");
                    await LoadViewData(vm);
                    return View(vm);
                }

                if (product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Sản phẩm '{product.Name}' chỉ còn {product.Quantity} sản phẩm trong kho.");
                    await LoadViewData(vm);
                    return View(vm);
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
                .Where(p => p.Status)
                .ToPagedListAsync(pageNumber, pageSize);

            return PartialView("_ProductListPartial", products); // OK
        }

        [Authorize(Roles = "Nhân viên bán hàng, Giám đốc")]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            if (order.Status == OrderStatus.Cancelled)
                return Json(new { success = false, message = "Đơn hàng này đã bị hủy trước đó." });

            return Json(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên bán hàng, Giám đốc")]
        public async Task<IActionResult> CancelConfirmed(int id, string cancelReason, string? customReason)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (order.Status == OrderStatus.Cancelled)
            {
                TempData["ErrorMessage"] = "Đơn hàng này đã bị hủy trước đó.";
                return RedirectToAction(nameof(Index));
            }

            // ✅ Xử lý lý do hủy
            if (cancelReason == "Khác")
            {
                cancelReason = !string.IsNullOrWhiteSpace(customReason)
                    ? $"Khác - {customReason}"
                    : "Khác";
            }

            // ✅ Hoàn tác số lượng sản phẩm
            foreach (var detail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(detail.ProductId);
                if (product != null)
                {
                    product.Quantity += detail.Quantity;
                }
            }

            // ✅ Cập nhật thông tin đơn hàng
            order.Status = OrderStatus.Cancelled;
            order.CancelReason = cancelReason;
            order.CancelledAt = DateTime.Now;
            order.CancelledBy = User.Identity?.Name ?? "Không xác định";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đơn hàng #{order.Id} đã được hủy thành công.";
            return RedirectToAction(nameof(Index));
        }

    }
}
