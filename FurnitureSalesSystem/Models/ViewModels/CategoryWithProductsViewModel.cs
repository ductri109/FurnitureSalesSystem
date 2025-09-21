namespace FurnitureSalesSystem.Models.ViewModels
{
    public class CategoryWithProductsViewModel
    {
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; } = new();
    }
}
