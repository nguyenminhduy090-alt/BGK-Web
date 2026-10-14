using Microsoft.EntityFrameworkCore;

namespace QuanLyMonAn.Data;

public static class MenuSeedTranslations
{
    public static async Task ApplyAsync(ApplicationDbContext context)
    {
        // Match both the original sample ID and English name. User-created or renamed dishes
        // are left alone. Running again after translation does not update the same rows again.
        await using var transaction = await context.Database.BeginTransactionAsync();
        await context.Database.ExecuteSqlRawAsync("""
            WITH translations (id, old_name, new_name, new_description) AS (
                VALUES
                    (1, 'Appetizer', 'Khai vị', 'Các món khai vị nhẹ'),
                    (2, 'Main Course', 'Món chính', 'Các món chính trong thực đơn'),
                    (3, 'Dessert', 'Tráng miệng', 'Các món ngọt dùng sau bữa ăn')
            )
            UPDATE "BIT240082_DishCategories" AS c
            SET "BIT240082_Name" = t.new_name,
                "BIT240082_Description" = t.new_description
            FROM translations AS t
            WHERE c."BIT240082_Id" = t.id
              AND c."BIT240082_Name" = t.old_name;

            WITH translations (id, old_name, new_name, new_description) AS (
                VALUES
                    (1, 'Spring Rolls', 'Chả giò', 'Chả giò chiên giòn'),
                    (2, 'Grilled Beef Salad', 'Salad bò nướng', 'Salad rau tươi kết hợp bò nướng'),
                    (3, 'Grilled Pork Chop Rice', 'Cơm sườn nướng', 'Cơm ăn kèm sườn heo nướng'),
                    (4, 'Beef Noodle Soup', 'Phở bò', 'Phở bò truyền thống'),
                    (5, 'Che Ba Mau', 'Chè ba màu', 'Món chè ngọt gồm ba màu')
            )
            UPDATE "BIT240082_Dishes" AS d
            SET "BIT240082_Name" = t.new_name,
                "BIT240082_Description" = t.new_description
            FROM translations AS t
            WHERE d."BIT240082_Id" = t.id
              AND d."BIT240082_Name" = t.old_name
              AND NOT EXISTS (
                  SELECT 1
                  FROM "BIT240082_Dishes" AS other
                  WHERE other."BIT240082_DishCategoryId" = d."BIT240082_DishCategoryId"
                    AND other."BIT240082_Name" = t.new_name
                    AND other."BIT240082_Id" <> d."BIT240082_Id"
              );
            """);
        await transaction.CommitAsync();
    }
}
