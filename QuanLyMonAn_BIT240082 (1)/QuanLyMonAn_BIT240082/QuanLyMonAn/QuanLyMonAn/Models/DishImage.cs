using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyMonAn.Models
{
    [Table("BIT240082_DishImages")]
    public class DishImage_BIT240082
    {
        [Column("BIT240082_Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Image URL is required.")]
        [StringLength(500, ErrorMessage = "Image URL must be at most 500 characters.")]
        [Column("BIT240082_ImageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("BIT240082_IsThumbnail")]
        public bool IsThumbnail { get; set; } = false;

        [Column("BIT240082_DishId")]
        public int DishId { get; set; }

        [ForeignKey("DishId")]
        public Dish_BIT240082? Dish { get; set; }
    }
}
