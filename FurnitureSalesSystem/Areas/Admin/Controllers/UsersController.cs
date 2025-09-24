using FurnitureSalesSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FurnitureSalesSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Giám đốc")]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index action called");

            var identityUsers = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in identityUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isLocked = await _userManager.IsLockedOutAsync(user);
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    IsLocked = isLocked,
                    LockoutEnd = lockoutEnd
                });
            }

            return View(userViewModels);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            _logger.LogInformation($"LockUser action called with id: {id}");

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("LockUser: ID is null or empty");
                TempData["Error"] = "ID người dùng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"LockUser: User not found with id: {id}");
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            // 🔒 Không cho khóa chính mình
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                _logger.LogWarning("LockUser: Attempt to lock own account.");
                TempData["Error"] = "Bạn không thể tự khóa tài khoản của chính mình.";
                return RedirectToAction("Index");
            }

            try
            {
                if (!user.LockoutEnabled)
                {
                    var enableResult = await _userManager.SetLockoutEnabledAsync(user, true);
                    if (!enableResult.Succeeded)
                    {
                        _logger.LogError($"LockUser: Failed to enable lockout for user {user.UserName}");
                        TempData["Error"] = "Không thể bật tính năng khóa tài khoản.";
                        return RedirectToAction("Index");
                    }
                }

                var lockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
                var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"LockUser: Successfully locked user {user.UserName}");
                    TempData["Success"] = $"Đã khóa tài khoản {user.UserName} thành công.";
                }
                else
                {
                    _logger.LogError($"LockUser: Failed to lock user {user.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    TempData["Error"] = "Khóa tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"LockUser: Exception occurred while locking user {user.UserName}");
                TempData["Error"] = "Có lỗi xảy ra khi khóa tài khoản.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            _logger.LogInformation($"UnlockUser action called with id: {id}");

            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("UnlockUser: ID is null or empty");
                TempData["Error"] = "ID người dùng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"UnlockUser: User not found with id: {id}");
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            _logger.LogInformation($"UnlockUser: Found user {user.UserName}");

            try
            {
                // Mở khóa tài khoản
                var result = await _userManager.SetLockoutEndDateAsync(user, null);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"UnlockUser: Successfully unlocked user {user.UserName}");
                    TempData["Success"] = $"Đã mở khóa tài khoản {user.UserName} thành công.";
                }
                else
                {
                    _logger.LogError($"UnlockUser: Failed to unlock user {user.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    TempData["Error"] = "Mở khóa tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UnlockUser: Exception occurred while unlocking user {user.UserName}");
                TempData["Error"] = "Có lỗi xảy ra khi mở khóa tài khoản.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(new UserCreateEditViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateEditViewModel model)
        {
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList(); // Luôn gán lại

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password!); // cần check mật khẩu null

            if (result.Succeeded)
            {
                if (model.Roles != null && model.Roles.Any())
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }

                return RedirectToAction(nameof(Index));
            }

            // THẤT BẠI -> hiện lỗi
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            // Không cho chỉnh sửa chính mình
            var currentUserId = _userManager.GetUserId(User);
            if (id == currentUserId)
            {
                TempData["Error"] = "Bạn không thể chỉnh sửa tài khoản của chính mình.";
                return RedirectToAction("Index");
            }
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserCreateEditViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                Password = null
                // Không hiển thị mật khẩu
            };

            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View(model);
            }
            var currentUserId = _userManager.GetUserId(User);
            if (model.Id == currentUserId)
            {
                TempData["Error"] = "Bạn không thể chỉnh sửa tài khoản của chính mình.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(model.Id!);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.UserName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật thông tin người dùng.");
                return View(model);
            }

            // Gán lại role
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (model.Roles != null)
                await _userManager.AddToRolesAsync(user, model.Roles);

            // Nếu mật khẩu khác "********" => đổi mật khẩu
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (!resetResult.Succeeded)
                {
                    foreach (var error in resetResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
                    return View(model);
                }
            }
            TempData["Success"] = "Cập nhật người dùng thành công.";
            return RedirectToAction("Index");
        }
    }
}