using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyMonAn.Models
{
    [Table("BIT240082_Dishes")]
    public class Dish_BIT240082
    {
        [Column("BIT240082_Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Dish name is required.")]
        [StringLength(150, ErrorMessage = "Dish name must be at most 150 characters.")]
        [Column("BIT240082_Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Column("BIT240082_Price", TypeName = "numeric(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Preparation time is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Preparation time must be greater than 0.")]
        [Column("BIT240082_PreparationTime")]
        public int PreparationTime { get; set; }

        [Column("BIT240082_IsAvailable")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(1000, ErrorMessage = "Description must be at most 1000 characters.")]
        [Column("BIT240082_Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Column("BIT240082_DishCategoryId")]
        public int DishCategoryId { get; set; }

        [ForeignKey("DishCategoryId")]
        public DishCategory_BIT240082? DishCategory { get; set; }

        public ICollection<DishImage_BIT240082> DishImages { get; set; } = new List<DishImage_BIT240082>();

        // Computed at display time only - NOT mapped to DB (Chức năng 3)
        [NotMapped]
        public string PreparationClassification =>
            PreparationTime <= 15 ? "Quick" : "Regular";
    }
}
