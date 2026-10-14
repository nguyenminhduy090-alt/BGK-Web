using Microsoft.EntityFrameworkCore;

namespace QuanLyMonAn.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Use migrations if the project contains them. Otherwise initialize this practical project
        // directly from its EF model, including the HasData sample records.
        if (context.Database.GetMigrations().Any())
        {
            await context.Database.MigrateAsync();
            return;
        }

        var created = await context.Database.EnsureCreatedAsync();
        if (!created) return;

        // HasData inserts explicit IDs. Advance PostgreSQL identity sequences so adding a new
        // category, dish or image cannot reuse the sample records' primary keys.
        await context.Database.ExecuteSqlRawAsync("""
            SELECT setval(pg_get_serial_sequence('"BIT240082_DishCategories"', 'BIT240082_Id'),
                (SELECT MAX("BIT240082_Id") FROM "BIT240082_DishCategories"), true);
            SELECT setval(pg_get_serial_sequence('"BIT240082_Dishes"', 'BIT240082_Id'),
                (SELECT MAX("BIT240082_Id") FROM "BIT240082_Dishes"), true);
            SELECT setval(pg_get_serial_sequence('"BIT240082_DishImages"', 'BIT240082_Id'),
                (SELECT MAX("BIT240082_Id") FROM "BIT240082_DishImages"), true);
            """);
    }
}
