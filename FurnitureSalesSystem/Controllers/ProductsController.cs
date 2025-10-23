using FurnitureSalesSystem.Data;
using FurnitureSalesSystem.Models;
using FurnitureSalesSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace FurnitureSalesSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        [Authorize(Roles = "Giám đốc, Nhân viên kho, Nhân viên bán hàng")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Status)
                .OrderByDescending(p => p.UpdatedAt)
                .ToPagedListAsync(pageNumber, pageSize);

            ViewBag.CategoryId = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            return View(products);
        }


        // GET: Products/Details/5
        [Authorize(Roles = "Giám đốc, Nhân viên kho, Nhân viên bán hàng")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Nhân viên kho")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên kho")]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile)
        {
            // ✅ TEST xem có nhận ảnh không
            Console.WriteLine($"[DEBUG] HasImage: {ImageFile != null}, Size: {ImageFile?.Length}, Name: {ImageFile?.FileName}");

            if (ModelState.IsValid)
            {
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    product.Image = "/images/products/" + fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        // GET: Products/Edit/5
        [Authorize(Roles = "Nhân viên kho")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên kho")]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null) return NotFound();

                    // Cập nhật các trường được phép sửa
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Quantity = product.Quantity;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.Description = product.Description;
                    existingProduct.UpdatedAt = DateTime.Now;

                    // ✅ Nếu có upload ảnh mới
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                        if (!Directory.Exists(imageFolder))
                            Directory.CreateDirectory(imageFolder);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(imageFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // Cập nhật đường dẫn ảnh
                        existingProduct.Image = "/images/products/" + fileName;
                    }

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        // GET: Products/Delete/5
        [Authorize(Roles = "Nhân viên kho")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nhân viên kho")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Status = false; // ❌ không xóa khỏi DB
                product.UpdatedAt = DateTime.Now;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Products/Overview
        [Authorize(Roles = "Giám đốc, Nhân viên kho, Nhân viên bán hàng")]
        public async Task<IActionResult> Overview()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            var viewModel = categories.Select(c => new CategoryWithProductsViewModel
            {
                CategoryName = c.Name,
                Products = c.Products.ToList()
            }).ToList();

            return View(viewModel);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
