using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyMonAn.Models
{
    [Table("BIT240082_DishCategories")]
    public class DishCategory_BIT240082
    {
        [Column("BIT240082_Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name must be at most 100 characters.")]
        [Column("BIT240082_Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
        [Column("BIT240082_Description")]
        public string? Description { get; set; }

        public ICollection<Dish_BIT240082> Dishes { get; set; } = new List<Dish_BIT240082>();
    }
}
