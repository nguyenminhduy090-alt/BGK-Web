using Microsoft.AspNetCore.Mvc;

namespace QuanLyMonAn.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dishes");
        }

        // Dùng để chuyển đổi ngôn ngữ - Chức năng 7
        [HttpGet]
        public IActionResult SetLanguage(string culture, string? returnUrl)
        {
            // Normalize UI language; keep numeric culture stable in both modes.
            culture = culture == "vi" || culture == "vi-VN" ? "vi" : "en";
            Response.Cookies.Append(
                Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.DefaultCookieName,
                Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.MakeCookieValue(
                    new Microsoft.AspNetCore.Localization.RequestCulture("en", culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true, HttpOnly = true, SameSite = SameSiteMode.Lax }
            );

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Dishes") ?? "/";
            }

            return LocalRedirect(returnUrl);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
