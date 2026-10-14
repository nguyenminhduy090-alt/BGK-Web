using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyMonAn.ViewModels
{
    public class DishFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Dish name is required.")]
        [StringLength(150, ErrorMessage = "Dish name must be at most 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Preparation time is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Preparation time must be greater than 0.")]
        public int PreparationTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int DishCategoryId { get; set; }

        // Optional first image entered when creating a dish.
        [Url(ErrorMessage = "Please enter a valid HTTP or HTTPS image URL.")]
        [StringLength(500, ErrorMessage = "Image URL must be at most 500 characters.")]
        public string? ImageUrl { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
