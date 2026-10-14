using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyMonAn.Models;

namespace QuanLyMonAn.ViewModels
{
    public class DishListViewModel
    {
        public List<Dish_BIT240082> Dishes { get; set; } = new();

        // Filter inputs (bound from query string => GET, tự giữ điều kiện sau submit)
        public string? SearchName { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsAvailable { get; set; } // null = tất cả, true = đang bán, false = ngừng bán
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } // "price_asc" | "price_desc" | "prep_asc"

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public string? ErrorMessage { get; set; }
    }
}
