using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CategoryValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;
using Newtonsoft.Json;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _env;

        public CategoryController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _env = env;
        }
        private async Task LoadDropdownsAsync(int? topCategoryId = null, int? subCategoryId = null)
        {
            var topCategories = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("topCategories/active");
            ViewBag.topCategories = topCategories?.Select(tc => new SelectListItem
            {
                Text = tc.Name,
                Value = tc.Id.ToString(),
                Selected = tc.Id == topCategoryId
            }).ToList() ?? new List<SelectListItem>();

            if (topCategoryId.HasValue)
            {
                var subCategories = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>($"subCategories/ByTopCategory/{topCategoryId}");
                ViewBag.subCategories = subCategories?.Select(sc => new SelectListItem
                {
                    Text = sc.Name,
                    Value = sc.Id.ToString(),
                    Selected = sc.Id == subCategoryId
                }).ToList() ?? new List<SelectListItem>();
            }
            else
            {
                ViewBag.subCategories = new List<SelectListItem>();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSubCategoriesByTopCategory(int topCategoryId)
        {
            var subCategories = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>($"subCategories/ByTopCategory/{topCategoryId}");
            if (subCategories == null || !subCategories.Any())
                return Content("<option value=''>Alt kategori bulunamadı</option>", "text/html");

            var options = subCategories.Select(sc => $"<option value='{sc.Id}'>{sc.Name}</option>").ToList();
            options.Insert(0, "<option value='' selected disabled>Alt kategori seçin</option>");
            return Content(string.Join("", options), "text/html");
        }

        public async Task<IActionResult> Index()
        {
            var resultDtos = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories/active");

            var vm = new CategoryViewModel
            {
                CategoryViewList = resultDtos
            };

            return View(vm);
        }


        public async Task<IActionResult> DeletedCategory()
        {
            ViewBag.ShowBackButton = true;
            var resultdeletedDtos = await _client.GetFromJsonAsync<List<ResultCategoryDto>>("categories/deleted");
            var vm = new CategoryViewModel
            {
                CategoryViewList = resultdeletedDtos
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            ViewBag.ShowBackButton = true;
            await LoadDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto dto)
        {
            var validator = new CreateCategoryValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                await LoadDropdownsAsync(dto.TopCategoryId, dto.SubCategoryId);
                return View(dto);
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // WebUI'de dosyayı kaydet ve WebP yap
                string imageUrl = await SaveImageAsWebPAsync(dto.ImageFile);
                dto.ImageUrl = imageUrl;
            }

            using var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(dto.Name ?? ""), "Name");
            formContent.Add(new StringContent(dto.TopCategoryId.ToString()), "TopCategoryId");
            if (dto.SubCategoryId.HasValue)
                formContent.Add(new StringContent(dto.SubCategoryId.Value.ToString()), "SubCategoryId");

            // WebUI’de işlenmiş WebP dosya ismini API’ye gönderiyoruz sadece, dosya değil
            formContent.Add(new StringContent(dto.ImageUrl ?? ""), "ImageUrl");

            var response = await _client.PostAsync("categories", formContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Kategori oluşturulamadı";

            await LoadDropdownsAsync(dto.TopCategoryId, dto.SubCategoryId);
            return View(dto);
        }

        public async Task<string> SaveImageAsWebPAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "categories");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + ".webp";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            ms.Position = 0;

            using Image<Rgba32> image = Image.Load<Rgba32>(ms);

            int maxWidth = 1280;
            int maxHeight = 720;

            var size = image.Size;

            if (size.Width > maxWidth || size.Height > maxHeight)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, maxHeight)
                }));
            }

            var encoder = new WebpEncoder { Quality = 75 };
            await image.SaveAsync(filePath, encoder);

            return "/images/categories/" + fileName;
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            ViewBag.ShowBackButton = true;

            var category = await _client.GetFromJsonAsync<UpdateCategoryDto>($"Categories/{id}");
            if (category == null) return NotFound();

            if ((category.TopCategoryId == 0 || category.TopCategoryId == null) && category.SubCategoryId > 0)
            {
                var subCategory = await _client.GetFromJsonAsync<ResultSubCategoryDto>($"SubCategories/{category.SubCategoryId}");
                if (subCategory != null)
                    category.TopCategoryId = subCategory.TopCategoryId;
            }

            ViewBag.ExistingImageUrl = category.ImageUrl;

            await LoadDropdownsAsync(category.TopCategoryId, category.SubCategoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto dto, string existingImageUrl)
        {
            var validator = new UpdateCategoryValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ShowBackButton = true;
                ViewBag.ExistingImageUrl = existingImageUrl;
                await LoadDropdownsAsync(dto.TopCategoryId, dto.SubCategoryId);
                return View(dto);
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // ✅ Yeni görsel geldiyse eskiyi fiziksel olarak sil
                if (!string.IsNullOrWhiteSpace(existingImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, existingImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // ✅ Yeni görseli WebP formatında kaydet
                var folderPath = Path.Combine(_env.WebRootPath, "images", "categories");
                Directory.CreateDirectory(folderPath);

                var newFileName = Guid.NewGuid() + ".webp";
                var newFilePath = Path.Combine(folderPath, newFileName);

                using var stream = dto.ImageFile.OpenReadStream();
                using var image = await Image.LoadAsync(stream);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(1280, 720)
                }));

                await image.SaveAsync(newFilePath, new WebpEncoder { Quality = 75 });
                dto.ImageUrl = "/images/categories/" + newFileName;
            }
            else
            {
                // 📌 Görsel gelmediyse mevcut görseli koru
                dto.ImageUrl = existingImageUrl;
            }

            var form = new MultipartFormDataContent
            {
                { new StringContent(dto.Id.ToString()), "Id" },
                { new StringContent(dto.Name ?? ""), "Name" },
                { new StringContent(dto.TopCategoryId.ToString()), "TopCategoryId" }
            };

            if (dto.SubCategoryId > 0)
                form.Add(new StringContent(dto.SubCategoryId.ToString()), "SubCategoryId");

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                form.Add(new StringContent(dto.ImageUrl), "ImageUrl");

            var response = await _client.PutAsync("categories", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Kategori güncellenemedi.";
            ViewBag.ExistingImageUrl = dto.ImageUrl;
            await LoadDropdownsAsync(dto.TopCategoryId, dto.SubCategoryId);
            return View(dto);
        }    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeletedCategory(int id)
        {
            var response = await _client.PostAsync($"Categories/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Kategori başarıyla soft silindi.";
            else
                TempData["ErrorMessage"] = "Kategori silinemedi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeletedCategory(int id)
        {
            var response = await _client.PostAsync($"Categories/undosoftdelete/{id}", null);

            if (response.IsSuccessStatusCode)
                TempData["SuccessMessage"] = "Kategori geri alındı.";
            else
                TempData["ErrorMessage"] = "Geri alma işlemi başarısız.";

            return RedirectToAction(nameof(DeletedCategory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteCategory(int id)
        {
            // Kategori bilgilerini al
            var categoryResponse = await _client.GetAsync($"categories/{id}");
            if (!categoryResponse.IsSuccessStatusCode)
                return BadRequest("Kategori bilgisi alınamadı.");

            var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<ResultCategoryDto>(categoryJson);

            // Kalıcı silme isteği
            var response = await _client.DeleteAsync($"categories/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            // API başarısızsa cevapla
            if (!response.IsSuccessStatusCode)
                return BadRequest(content);

            // Görsel varsa fiziksel olarak sil
            if (!string.IsNullOrWhiteSpace(category.ImageUrl))
            {
                var relativePath = category.ImageUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
                var imagePath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            return Ok(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("categories/DeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("categories/UndoSoftDeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç kategori seçilmedi.");

            // Tüm kategori bilgilerini topla
            var categoryImages = new List<string>();

            foreach (var id in ids)
            {
                var detailResponse = await _client.GetAsync($"categories/{id}");
                if (!detailResponse.IsSuccessStatusCode)
                    continue; // Bilgi alınamazsa görseli silmeyi deneme

                var json = await detailResponse.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<ResultCategoryDto>(json);

                if (!string.IsNullOrWhiteSpace(category.ImageUrl))
                    categoryImages.Add(category.ImageUrl);
            }

            // API'ye kalıcı silme isteği gönder
            var response = await _client.PostAsJsonAsync("categories/PermanentDeleteMultiple", ids);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest(content);

            // Görsel dosyalarını fiziksel olarak sil
            foreach (var imageUrl in categoryImages)
            {
                var relativePath = imageUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
                var imagePath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            return Ok(content);
        }

    }
}
