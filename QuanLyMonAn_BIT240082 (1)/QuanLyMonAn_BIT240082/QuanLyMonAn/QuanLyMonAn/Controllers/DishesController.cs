using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyMonAn.Data;
using QuanLyMonAn.Models;
using QuanLyMonAn.Resources;
using Microsoft.Extensions.Localization;
using QuanLyMonAn.ViewModels;

namespace QuanLyMonAn.Controllers
{
    public class DishesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DishesController(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: Dishes
        // Chức năng 2: tìm kiếm + lọc + sắp xếp kết hợp đồng thời, truy vấn ở tầng DB, giữ điều kiện qua query string (GET)
        public async Task<IActionResult> Index(DishListViewModel filter)
        {
            string? error = null;

            // Validate khoảng giá không hợp lệ (Chức năng 6)
            if (filter.MinPrice.HasValue && filter.MinPrice < 0)
                error = "Minimum price cannot be negative.";
            else if (filter.MaxPrice.HasValue && filter.MaxPrice < 0)
                error = "Maximum price cannot be negative.";
            else if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue && filter.MinPrice > filter.MaxPrice)
                error = "Minimum price cannot be greater than maximum price.";

            IQueryable<Dish_BIT240082> query = _context.Dishes
                .Include(d => d.DishCategory)
                .Include(d => d.DishImages);

            if (error == null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchName))
                    query = query.Where(d => d.Name.Contains(filter.SearchName));

                if (filter.CategoryId.HasValue)
                    query = query.Where(d => d.DishCategoryId == filter.CategoryId.Value);

                if (filter.IsAvailable.HasValue)
                    query = query.Where(d => d.IsAvailable == filter.IsAvailable.Value);

                if (filter.MinPrice.HasValue)
                    query = query.Where(d => d.Price >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    query = query.Where(d => d.Price <= filter.MaxPrice.Value);

                query = filter.SortBy switch
                {
                    "price_desc" => query.OrderByDescending(d => d.Price),
                    "prep_asc" => query.OrderBy(d => d.PreparationTime),
                    _ => query.OrderBy(d => d.Price), // "price_asc" mặc định
                };

                filter.Dishes = await query.ToListAsync();
            }
            else
            {
                filter.Dishes = new List<Dish_BIT240082>();
            }

            filter.ErrorMessage = error;
            filter.Categories = await GetCategorySelectListAsync(filter.CategoryId);

            return View(filter);
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dish = await _context.Dishes
                .Include(d => d.DishCategory)
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null) return NotFound();

            return View(dish);
        }

        // GET: Dishes/Create
        public async Task<IActionResult> Create()
        {
            var vm = new DishFormViewModel
            {
                IsAvailable = true,
                Categories = await GetCategorySelectListAsync(null)
            };
            return View(vm);
        }

        // POST: Dishes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DishFormViewModel vm)
        {
            await ValidateCategoryExistsAsync(vm.DishCategoryId);
            await ValidateDuplicateNameAsync(vm.Name, vm.DishCategoryId, null);

            var imageErrors = ModelState[nameof(vm.ImageUrl)]?.Errors.Count ?? 0;
            if (imageErrors == 0 && !string.IsNullOrWhiteSpace(vm.ImageUrl))
            {
                if (!Uri.TryCreate(vm.ImageUrl.Trim(), UriKind.Absolute, out var imageUri) ||
                    (imageUri.Scheme != Uri.UriSchemeHttp && imageUri.Scheme != Uri.UriSchemeHttps))
                {
                    ModelState.AddModelError(nameof(vm.ImageUrl),
                        _localizer["Please enter a valid HTTP or HTTPS image URL."].Value);
                }
            }

            if (ModelState.IsValid)
            {
                var dish = new Dish_BIT240082
                {
                    Name = vm.Name,
                    Price = vm.Price,
                    PreparationTime = vm.PreparationTime,
                    IsAvailable = vm.IsAvailable,
                    Description = vm.Description,
                    DishCategoryId = vm.DishCategoryId
                };

                if (!string.IsNullOrWhiteSpace(vm.ImageUrl))
                {
                    dish.DishImages.Add(new DishImage_BIT240082
                    {
                        ImageUrl = vm.ImageUrl.Trim(),
                        IsThumbnail = true
                    });
                }

                // EF saves the dish and its first image in one transaction and fills DishId.
                _context.Add(dish);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty,
                        _localizer["Unable to save the dish. Please check the data and try again."].Value);
                    vm.Categories = await GetCategorySelectListAsync(vm.DishCategoryId);
                    return View(vm);
                }
                TempData["SuccessMessage"] = "Dish created successfully.";
                return RedirectToAction(nameof(Index));
            }

            vm.Categories = await GetCategorySelectListAsync(vm.DishCategoryId);
            return View(vm);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            var vm = new DishFormViewModel
            {
                Id = dish.Id,
                Name = dish.Name,
                Price = dish.Price,
                PreparationTime = dish.PreparationTime,
                IsAvailable = dish.IsAvailable,
                Description = dish.Description,
                DishCategoryId = dish.DishCategoryId,
                Categories = await GetCategorySelectListAsync(dish.DishCategoryId)
            };

            return View(vm);
        }

        // POST: Dishes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DishFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            await ValidateCategoryExistsAsync(vm.DishCategoryId);
            await ValidateDuplicateNameAsync(vm.Name, vm.DishCategoryId, vm.Id);

            if (ModelState.IsValid)
            {
                var dish = await _context.Dishes.FindAsync(id);
                if (dish == null) return NotFound();

                dish.Name = vm.Name;
                dish.Price = vm.Price;
                dish.PreparationTime = vm.PreparationTime;
                dish.IsAvailable = vm.IsAvailable;
                dish.Description = vm.Description;
                dish.DishCategoryId = vm.DishCategoryId;

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Dish updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Dishes.Any(d => d.Id == id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            vm.Categories = await GetCategorySelectListAsync(vm.DishCategoryId);
            return View(vm);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dish = await _context.Dishes
                .Include(d => d.DishCategory)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null) return NotFound();

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Dish deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ============== QUẢN LÝ ẢNH (Chức năng 4) ==============

        // POST: Dishes/AddImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int dishId, string imageUrl, bool setAsThumbnail)
        {
            var dish = await _context.Dishes
                .Include(d => d.DishImages)
                .FirstOrDefaultAsync(d => d.Id == dishId);

            if (dish == null) return NotFound();

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                TempData["ErrorMessage"] = "Image URL is required.";
                return RedirectToAction(nameof(Details), new { id = dishId });
            }

            if (setAsThumbnail || !dish.DishImages.Any())
            {
                foreach (var img in dish.DishImages)
                    img.IsThumbnail = false;
            }

            var newImage = new DishImage_BIT240082
            {
                ImageUrl = imageUrl.Trim(),
                DishId = dishId,
                // Ảnh đầu tiên của món tự động là thumbnail; nếu đánh dấu rõ ràng thì set true
                IsThumbnail = setAsThumbnail || !dish.DishImages.Any()
            };

            _context.DishImages.Add(newImage);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Image added successfully.";
            return RedirectToAction(nameof(Details), new { id = dishId });
        }

        // POST: Dishes/SetThumbnail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetThumbnail(int imageId, int dishId)
        {
            var images = await _context.DishImages.Where(i => i.DishId == dishId).ToListAsync();
            var target = images.FirstOrDefault(i => i.Id == imageId);
            if (target == null) return NotFound();

            // Đảm bảo mỗi món tối đa 1 ảnh IsThumbnail = true
            foreach (var img in images)
                img.IsThumbnail = (img.Id == imageId);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thumbnail updated.";
            return RedirectToAction(nameof(Details), new { id = dishId });
        }

        // POST: Dishes/DeleteImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int dishId)
        {
            var image = await _context.DishImages.FirstOrDefaultAsync(i => i.Id == imageId && i.DishId == dishId);
            if (image == null) return NotFound();

            bool wasThumbnail = image.IsThumbnail;
            _context.DishImages.Remove(image);
            await _context.SaveChangesAsync();

            // Nếu xóa ảnh đại diện, tự động gán ảnh còn lại đầu tiên làm thumbnail mới (nếu còn ảnh)
            if (wasThumbnail)
            {
                var remaining = await _context.DishImages.Where(i => i.DishId == dishId).FirstOrDefaultAsync();
                if (remaining != null)
                {
                    remaining.IsThumbnail = true;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Image deleted.";
            return RedirectToAction(nameof(Details), new { id = dishId });
        }

        // ============== HELPERS ==============

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync(int? selectedId)
        {
            var categories = await _context.DishCategories.OrderBy(c => c.Name).ToListAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = selectedId.HasValue && c.Id == selectedId.Value
            });
        }

        private async Task ValidateCategoryExistsAsync(int categoryId)
        {
            bool exists = await _context.DishCategories.AnyAsync(c => c.Id == categoryId);
            if (!exists)
                ModelState.AddModelError(nameof(DishFormViewModel.DishCategoryId), _localizer["Selected category does not exist."].Value);
        }

        private async Task ValidateDuplicateNameAsync(string name, int categoryId, int? excludeId)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            bool duplicate = await _context.Dishes.AnyAsync(d =>
                d.DishCategoryId == categoryId &&
                d.Name.ToLower() == name.Trim().ToLower() &&
                (!excludeId.HasValue || d.Id != excludeId.Value));

            if (duplicate)
                ModelState.AddModelError(nameof(DishFormViewModel.Name), _localizer["A dish with this name already exists in this category."].Value);
        }
    }
}
