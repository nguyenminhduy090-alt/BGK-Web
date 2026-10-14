using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using QuanLyMonAn.Data;
using QuanLyMonAn.Resources;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Localization (Chức năng 7: English mặc định, chuyển đổi English <-> Tiếng Việt)
// SharedResource already has namespace QuanLyMonAn.Resources.
// Adding ResourcesPath again would look for Resources/Resources/SharedResource.vi.resx.
builder.Services.AddLocalization();

builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

var app = builder.Build();

// Request localization: mặc định English, hỗ trợ English + Tiếng Việt, lưu lựa chọn qua cookie
// UI language can change independently of numeric form parsing and price display.
var supportedCultures = new[] { new CultureInfo("en") };
var supportedUiCultures = new[] { new CultureInfo("en"), new CultureInfo("vi") };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en", "en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedUiCultures
};
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new CookieRequestCultureProvider()
};

app.UseRequestLocalization(localizationOptions);

// Khi Controller trả về NotFound() (404), re-execute sang trang lỗi thân thiện có đa ngôn ngữ
app.UseStatusCodePagesWithReExecute("/Home/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dishes}/{action=Index}/{id?}");

// Prepare the database before serving the first request.
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DatabaseInitializer.InitializeAsync(database);
    await MenuSeedTranslations.ApplyAsync(database);
}

app.Run();
