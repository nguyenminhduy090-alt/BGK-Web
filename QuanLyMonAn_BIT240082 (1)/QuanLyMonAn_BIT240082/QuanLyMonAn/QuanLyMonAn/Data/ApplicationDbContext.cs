using Microsoft.EntityFrameworkCore;
using QuanLyMonAn.Models;

namespace QuanLyMonAn.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<DishCategory_BIT240082> DishCategories { get; set; } = null!;
        public DbSet<Dish_BIT240082> Dishes { get; set; } = null!;
        public DbSet<DishImage_BIT240082> DishImages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DishCategory 1 - n Dish, RESTRICT delete => không cho xóa loại đang có món (Chức năng 5)
            modelBuilder.Entity<Dish_BIT240082>()
                .HasOne(d => d.DishCategory)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.DishCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dish 1 - n DishImage, cascade delete ảnh khi xóa món
            modelBuilder.Entity<DishImage_BIT240082>()
                .HasOne(i => i.Dish)
                .WithMany(d => d.DishImages)
                .HasForeignKey(i => i.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tên món không trùng trong cùng 1 loại
            modelBuilder.Entity<Dish_BIT240082>()
                .HasIndex(d => new { d.DishCategoryId, d.Name })
                .IsUnique();

            // ================= SEED DATA =================
            modelBuilder.Entity<DishCategory_BIT240082>().HasData(
                new DishCategory_BIT240082 { Id = 1, Name = "Khai vị", Description = "Các món khai vị nhẹ" },
                new DishCategory_BIT240082 { Id = 2, Name = "Món chính", Description = "Các món chính trong thực đơn" },
                new DishCategory_BIT240082 { Id = 3, Name = "Tráng miệng", Description = "Các món ngọt dùng sau bữa ăn" }
            );

            modelBuilder.Entity<Dish_BIT240082>().HasData(
                new Dish_BIT240082 { Id = 1, Name = "Chả giò", Price = 45000, PreparationTime = 10, IsAvailable = true, Description = "Chả giò chiên giòn", DishCategoryId = 1 },
                new Dish_BIT240082 { Id = 2, Name = "Salad bò nướng", Price = 65000, PreparationTime = 15, IsAvailable = true, Description = "Salad rau tươi kết hợp bò nướng", DishCategoryId = 1 },
                new Dish_BIT240082 { Id = 3, Name = "Cơm sườn nướng", Price = 55000, PreparationTime = 20, IsAvailable = true, Description = "Cơm ăn kèm sườn heo nướng", DishCategoryId = 2 },
                new Dish_BIT240082 { Id = 4, Name = "Phở bò", Price = 60000, PreparationTime = 25, IsAvailable = false, Description = "Phở bò truyền thống", DishCategoryId = 2 },
                new Dish_BIT240082 { Id = 5, Name = "Chè ba màu", Price = 25000, PreparationTime = 8, IsAvailable = true, Description = "Món chè ngọt gồm ba màu", DishCategoryId = 3 }
            );

            modelBuilder.Entity<DishImage_BIT240082>().HasData(
                new DishImage_BIT240082 { Id = 1, ImageUrl = "https://picsum.photos/seed/springrolls/400/300", IsThumbnail = true, DishId = 1 },
                new DishImage_BIT240082 { Id = 2, ImageUrl = "https://picsum.photos/seed/beefsalad/400/300", IsThumbnail = true, DishId = 2 },
                new DishImage_BIT240082 { Id = 3, ImageUrl = "https://picsum.photos/seed/porkchop1/400/300", IsThumbnail = true, DishId = 3 },
                new DishImage_BIT240082 { Id = 4, ImageUrl = "https://picsum.photos/seed/porkchop2/400/300", IsThumbnail = false, DishId = 3 },
                new DishImage_BIT240082 { Id = 5, ImageUrl = "https://picsum.photos/seed/beefnoodle/400/300", IsThumbnail = true, DishId = 4 },
                new DishImage_BIT240082 { Id = 6, ImageUrl = "https://picsum.photos/seed/cheba/400/300", IsThumbnail = true, DishId = 5 }
            );
        }
    }
}
