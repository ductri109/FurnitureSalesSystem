using FurnitureSalesSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureSalesSystem.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Xóa tất cả người dùng
            foreach (var user in userManager.Users.ToList())
            {
                await userManager.DeleteAsync(user);
            }

            // 2. Xóa tất cả role
            foreach (var role in roleManager.Roles.ToList())
            {
                await roleManager.DeleteAsync(role);
            }

            // 3. Tạo lại role
            string[] roles = { "Giám đốc", "Nhân viên kho", "Nhân viên bán hàng" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 4. Tạo admin (Giám đốc)
            var adminEmail = "admin@amlive.com";
            var adminUser = new IdentityUser
            {
                UserName = "giamdoc",
                Email = adminEmail,
                EmailConfirmed = true,
                LockoutEnabled = true
            };
            await userManager.CreateAsync(adminUser, "GiamDoc123@");
            await userManager.AddToRoleAsync(adminUser, "Giám đốc");

            // 6. Tạo warehouse (Nhân viên kho)
            var warehouseEmail = "warehouse@amlive.com";
            var warehouseUser = new IdentityUser
            {
                UserName = "kho",
                Email = warehouseEmail,
                EmailConfirmed = true,
                LockoutEnabled = true,
            };
            await userManager.CreateAsync(warehouseUser, "Kho123@");
            await userManager.AddToRoleAsync(warehouseUser, "Nhân viên kho");

            // 7. Tạo sell (Nhân viên bán hàng)
            var sellEmail = "sell@amlive.com";
            var sellUser = new IdentityUser
            {
                UserName = "banhang",
                Email = sellEmail,
                EmailConfirmed = true,
                LockoutEnabled = true,
            };
            await userManager.CreateAsync(sellUser, "BanHang123@");
            await userManager.AddToRoleAsync(sellUser, "Nhân viên bán hàng");
        }


        public static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Bàn" },
                    new Category { Name = "Ghế" },
                    new Category { Name = "Tủ" }
                );
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (context.Products.Any())
            {
                context.Products.RemoveRange(context.Products);
                await context.SaveChangesAsync();
            }
            var products = new List<Product>
                {
                    new Product { Name = "Bàn trà hình Ovan 2 tầng", Price = 399000, Quantity = 5, CategoryId = 11, Image = "/images/products/AL001.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn sofa gỗ nhỏ có hộc để đồ", Price = 4500000, Quantity = 12, CategoryId = 11, Image = "/images/products/AL002.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trà đạo bệt kiểu Nhật", Price = 1455000, Quantity = 20, CategoryId = 11, Image = "/images/products/AL003.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bộ bàn ăn 4 ghế gỗ sồi cổ điển", Price = 13200000, Quantity = 18, CategoryId = 15, Image = "/images/products/AL004.jpeg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn ăn đá Ceramic sang trọng", Price = 2250000, Quantity = 13, CategoryId = 15, Image = "/images/products/AL005.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn ăn thông minh gấp gọn", Price = 915000, Quantity = 18, CategoryId = 15, Image = "/images/products/AL006.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bộ bàn ăn tròn và 4 ghế sừng trâu", Price = 5200000, Quantity = 10, CategoryId = 15, Image = "/images/products/AL007.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn làm việc có ngăn kéo tủ", Price = 2800000, Quantity = 11, CategoryId = 14, Image = "/images/products/AL008.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn làm việc chữ L gỗ sồi", Price = 6800000, Quantity = 7, CategoryId = 14, Image = "/images/products/AL009.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn làm việc di động điều chỉnh độ cao", Price = 2400000, Quantity = 9, CategoryId = 14, Image = "/images/products/AL010.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn làm việc kết hợp giá sách", Price = 1950000, Quantity = 9, CategoryId = 14, Image = "/images/products/AL011.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn làm việc gỗ", Price = 780000, Quantity = 9, CategoryId = 14, Image = "/images/products/AL012.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bộ đôi bàn trà Reno nhám", Price = 6900000, Quantity = 20, CategoryId = 11, Image = "/images/products/AL013.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trà gỗ cao cấp", Price = 2000000, Quantity = 12, CategoryId = 11, Image = "/images/products/AL014.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn tab gỗ đầu giường có ngăn kéo", Price = 2890000, Quantity = 15, CategoryId = 17, Image = "/images/products/AL015.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn console trang trí gỗ cao cấp", Price = 6500000, Quantity = 12, CategoryId = 17, Image = "/images/products/AL016.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trang trí tròn mặt đá", Price = 4200000, Quantity = 5, CategoryId = 17, Image = "/images/products/AL017.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn console trang trí chân inox mạ vàng", Price = 3800000, Quantity = 14, CategoryId = 17, Image = "/images/products/AL018.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trang điểm kèm gương và ngăn kéo", Price = 3500000, Quantity = 14, CategoryId = 19, Image = "/images/products/AL019.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trang điểm mini bọc da màu hồng", Price = 3800000, Quantity = 17, CategoryId = 19, Image = "/images/products/AL020.webp", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trang điểm gỗ sồi nhám", Price = 4200000, Quantity = 14, CategoryId = 19, Image = "/images/products/AL021.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trang điểm treo tường", Price = 1300000, Quantity = 15, CategoryId = 19, Image = "/images/products/AL022.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bàn trang điểm mini", Price = 2200000, Quantity = 7, CategoryId = 19, Image = "/images/products/AL023.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Ghế sofa đơn xám nhạt", Price = 1290000, Quantity = 13, CategoryId = 10, Image = "/images/products/AL024.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Ghế sofa băng dài", Price = 5500000, Quantity = 18, CategoryId = 10, Image = "/images/products/AL025.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Ghế sofa bọc vải nhung", Price = 8800000, Quantity = 10, CategoryId = 10, Image = "/images/products/AL026.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Bộ ghế sofa bí ngô", Price = 6000000, Quantity = 16, CategoryId = 10, Image = "/images/products/AL027.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Ghế bập bênh nằm thư giãn", Price = 280000, Quantity = 10, CategoryId = 17, Image = "/images/products/AL028.jpeg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Ghế đôn chibi decor phòng ngủ", Price = 190000, Quantity = 8, CategoryId = 17, Image = "/images/products/AL029.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Ghế đẩu mini có kệ ở dưới", Price = 220000, Quantity = 18, CategoryId = 17, Image = "/images/products/AL030.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Tủ quần áo cánh kinh hiện đại", Price = 4290000, Quantity = 9, CategoryId = 13, Image = "/images/products/AL031.jpg" , Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình."},
                    new Product { Name = "Tủ quần áo kính đen mờ khung nhôm gỗ", Price = 3990000, Quantity = 17, CategoryId = 13, Image = "/images/products/AL032.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Tủ quần áo gỗ 2 cánh", Price = 2800000, Quantity = 15, CategoryId = 13, Image = "/images/products/AL033.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Tủ quần áo 2 cánh gỗ sồi", Price = 2560000, Quantity = 9, CategoryId = 13, Image = "/images/products/AL034.jpg" , Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình."},
                    new Product { Name = "Kệ tủ gỗ trang trí 5 tầng", Price = 5200000, Quantity = 15, CategoryId = 17, Image = "/images/products/AL035.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ tủ trang trí phòng khách gỗ cao su", Price = 3800000, Quantity = 6, CategoryId = 17, Image = "/images/products/AL036.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ gỗ trang trí tích hợp ngăn kéo", Price = 3580000, Quantity = 7, CategoryId = 17, Image = "/images/products/AL037.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ sách góc tường vuông 5 tầng", Price = 790000, Quantity = 8, CategoryId = 18, Image = "/images/products/AL038.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ sách treo góc 4 tầng trang trí", Price = 460000, Quantity = 7, CategoryId = 18, Image = "/images/products/AL039.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ sách đứng 6 tầng gỗ khung sắt", Price = 1270000, Quantity = 14, CategoryId = 18, Image = "/images/products/AL040.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ sách đứng hình cây gỗ cao su", Price = 2750000, Quantity = 10, CategoryId = 18, Image = "/images/products/AL041.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình."},
                    new Product { Name = "Kệ sách để bàn 2 tầng có ngăn kéo", Price = 890000, Quantity = 9, CategoryId = 18, Image = "/images/products/AL042.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Giường ngủ tích hợp ngăn kéo và kệ", Price = 1850000, Quantity = 20, CategoryId = 12, Image = "/images/products/AL043.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Giường ngủ thấp gỗ hiện đại", Price = 3600000, Quantity = 12, CategoryId = 12, Image = "/images/products/AL044.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình."},
                    new Product { Name = "Giường ngủ bệt kiểu Nhật", Price = 4250000, Quantity = 17, CategoryId = 12, Image = "/images/products/AL045.png", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Giường ngủ gỗ trắng nhỏ có ngăn kéo", Price = 990000, Quantity = 19, CategoryId = 12, Image = "/images/products/AL046.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Giường ngủ gỗ tròn sang trọng", Price = 5190000, Quantity = 11, CategoryId = 12, Image = "/images/products/AL047.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Giường tầng khung sắt hiện đại", Price = 1950000, Quantity = 17, CategoryId = 12, Image = "/images/products/AL048.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ tivi gỗ sồi tích hợp ngăn kéo để đồ", Price = 9800000, Quantity = 15, CategoryId = 21, Image = "/images/products/AL049.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ tivi vân gỗ tự nhiên với 4 ngăn kéo", Price = 2990000, Quantity = 5, CategoryId = 21, Image = "/images/products/AL050.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ tivi gỗ óc chó", Price = 16500000, Quantity = 20, CategoryId = 21, Image = "/images/products/AL051.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Kệ tivi treo tường gỗ thông tự nhiên", Price = 8700000, Quantity = 6, CategoryId = 21, Image = "/images/products/AL052.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Đèn ngủ để bàn bóng tròn", Price = 700000, Quantity = 19, CategoryId = 17, Image = "/images/products/AL053.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Đèn ngủ để bàn thiết kế thuỷ tinh", Price = 1200000, Quantity = 18, CategoryId = 17, Image = "/images/products/AL054.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Đèn cây trang trí tích hợp mặt bàn nhỏ", Price = 3250000, Quantity = 11, CategoryId = 17, Image = "/images/products/AL055.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Đèn ngủ cây để phòng ngủ", Price = 1680000, Quantity = 8, CategoryId = 17, Image = "/images/products/AL056.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Đèn rọi gắn tường đọc sách", Price = 230000, Quantity = 14, CategoryId = 17, Image = "/images/products/AL057.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Tủ giày dép gỗ kiểu dáng hiện đại", Price = 3700000, Quantity = 8, CategoryId = 20, Image = "/images/products/AL058.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Tủ giày gỗ 4 cánh cho gia đình", Price = 3400000, Quantity = 8, CategoryId = 20, Image = "/images/products/AL059.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    new Product { Name = "Tủ giày gỗ 3 tầng", Price = 2800000, Quantity = 13, CategoryId = 20, Image = "/images/products/AL060.jpg", Description = "Chiếc bàn sofa nhỏ gọn với thiết kế tinh tế, được làm hoàn toàn từ gỗ tự nhiên chắc chắn. Bàn có hộc để đồ rộng rãi, giúp bạn dễ dàng cất giữ remote, tạp chí hay những món đồ nhỏ trong phòng khách. Với kiểu dáng thanh lịch và màu gỗ trầm sang trọng, sản phẩm mang đến sự ấm áp và tiện nghi cho không gian sinh hoạt của gia đình." },
                    };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        public static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            if (context.Customers.Any())
            {
                context.Customers.RemoveRange(context.Customers);
                await context.SaveChangesAsync();
            }

            var customers = new List<Customer>
            {
                new Customer { FullName = "Nguyễn Văn An", Email = "an.nguyen@example.com", Phone = "0911000001", Address = "123 Lê Lợi, Hà Nội" },
                new Customer { FullName = "Trần Thị Bình", Email = "binh.tran@example.com", Phone = "0911000002", Address = "456 Trần Phú, TP.HCM" },
                new Customer { FullName = "Lê Văn Cường", Email = "cuong.le@example.com", Phone = "0911000003", Address = "789 Nguyễn Huệ, Đà Nẵng" },
                new Customer { FullName = "Phạm Thị Dung", Email = "dung.pham@example.com", Phone = "0911000004", Address = "321 Hùng Vương, Cần Thơ" },
                new Customer { FullName = "Hoàng Văn Dũng", Email = "dung.hoang@example.com", Phone = "0911000005", Address = "654 Điện Biên Phủ, Hải Phòng" },
                new Customer { FullName = "Vũ Thị Hà", Email = "ha.vu@example.com", Phone = "0911000006", Address = "987 Lý Thường Kiệt, Nha Trang" },
                new Customer { FullName = "Đỗ Văn Hùng", Email = "hung.do@example.com", Phone = "0911000007", Address = "147 Nguyễn Trãi, Bình Dương" },
                new Customer { FullName = "Bùi Thị Hương", Email = "huong.bui@example.com", Phone = "0911000008", Address = "258 Lê Văn Sỹ, Vũng Tàu" },
                new Customer { FullName = "Ngô Văn Khánh", Email = "khanh.ngo@example.com", Phone = "0911000009", Address = "369 Phan Đình Phùng, Huế" },
                new Customer { FullName = "Đặng Thị Lan", Email = "lan.dang@example.com", Phone = "0911000010", Address = "159 Pasteur, Đà Lạt" },
                new Customer { FullName = "Nguyễn Văn Long", Email = "long.nguyen@example.com", Phone = "0911000011", Address = "753 Hai Bà Trưng, Hà Nội" },
                new Customer { FullName = "Trần Thị Mai", Email = "mai.tran@example.com", Phone = "0911000012", Address = "852 Trường Chinh, TP.HCM" },
                new Customer { FullName = "Lê Văn Minh", Email = "minh.le@example.com", Phone = "0911000013", Address = "963 Nguyễn Văn Cừ, Đà Nẵng" },
                new Customer { FullName = "Phạm Thị Nga", Email = "nga.pham@example.com", Phone = "0911000014", Address = "357 Hoàng Diệu, Hải Dương" },
                new Customer { FullName = "Hoàng Văn Nam", Email = "nam.hoang@example.com", Phone = "0911000015", Address = "951 Cách Mạng Tháng Tám, Cần Thơ" },
                new Customer { FullName = "Vũ Thị Phương", Email = "phuong.vu@example.com", Phone = "0911000016", Address = "753 Võ Thị Sáu, Đồng Nai" },
                new Customer { FullName = "Đỗ Văn Quang", Email = "quang.do@example.com", Phone = "0911000017", Address = "258 Quang Trung, Quảng Ninh" },
                new Customer { FullName = "Bùi Thị Quyên", Email = "quyen.bui@example.com", Phone = "0911000018", Address = "147 Lê Duẩn, Thanh Hóa" },
                new Customer { FullName = "Ngô Văn Sơn", Email = "son.ngo@example.com", Phone = "0911000019", Address = "369 Lý Tự Trọng, Bắc Ninh" },
                new Customer { FullName = "Đặng Thị Thủy", Email = "thuy.dang@example.com", Phone = "0911000020", Address = "456 Nguyễn Hữu Thọ, Hà Tĩnh" }
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();
        }


    }
}
