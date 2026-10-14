using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyMonAn.Data;
using QuanLyMonAn.Models;
using QuanLyMonAn.Resources;
using Microsoft.Extensions.Localization;

namespace QuanLyMonAn.Controllers
{
    public class DishCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DishCategoriesController(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: DishCategories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.DishCategories
                .Include(c => c.Dishes)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // GET: DishCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.DishCategories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // GET: DishCategories/Create
        public IActionResult Create()
        {
            return View(new DishCategory_BIT240082());
        }

        // POST: DishCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] DishCategory_BIT240082 category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: DishCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.DishCategories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: DishCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] DishCategory_BIT240082 category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DishCategories.Any(c => c.Id == category.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: DishCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.DishCategories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // POST: DishCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.DishCategories
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            // Ràng buộc nghiệp vụ: không cho xóa loại đang có món sử dụng (Chức năng 5)
            if (category.Dishes.Any())
            {
                TempData["ErrorMessage"] =
                    _localizer["Cannot delete category \"{0}\" because it still has {1} dish(es) using it.", category.Name, category.Dishes.Count].Value;
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                _context.DishCategories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Cannot delete this category because it is still referenced by other data.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
