using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.DiscountDtos;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.DiscountValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.FinanceValidatorDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ProductValidatorDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using CleaningSuppliesSystem.WebUI.Helpers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System.Drawing;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _env;

        public ProductController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _env = env;
        }
        private async Task LoadBrandsDropdownAsync(int? selectedBrandId = null)
        {
            var brands = await _client.GetFromJsonAsync<List<ResultBrandDto>>("brands/active");
            ViewBag.brands = new SelectList(brands, "Id", "Name", selectedBrandId);
        }
        public async Task<IActionResult> Index()
        {
            var resultDtos = await _client.GetFromJsonAsync<List<ResultProductDto>>("products/active");

            var vm = new ProductViewModel
            {
                ProductViewList = resultDtos
            };

            return View(vm);
        }

        public async Task<IActionResult> DeletedProduct()
        {
            ViewBag.ShowBackButton = true;
            var resultdeletedDtos = await _client.GetFromJsonAsync<List<ResultProductDto>>("products/deleted");
            var vm = new ProductViewModel
            {
                ProductViewList = resultdeletedDtos
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.ShowBackButton = true;
            await LoadBrandsDropdownAsync();
            return View();
        }


        public async Task<string> SaveImageAsWebPAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");

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

            return "/images/products/" + fileName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            var validator = new CreateProductValidator();
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                await LoadBrandsDropdownAsync(dto.BrandId);
                return View(dto);
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                string imageUrl = await SaveImageAsWebPAsync(dto.ImageFile);
                dto.ImageUrl = imageUrl;
            }

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(dto.Name ?? ""), "Name");
            form.Add(new StringContent(dto.BrandId.ToString()), "BrandId");
            form.Add(new StringContent(dto.UnitPrice.ToString()), "UnitPrice");
            form.Add(new StringContent(dto.ImageUrl ?? ""), "ImageUrl");

            var response = await _client.PostAsync("products", form);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Ürün oluşturulurken bir hata oluştu.");
                await LoadBrandsDropdownAsync(dto.BrandId);
                return View(dto);
            }

            TempData["SuccessMessage"] = "Ürün başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> UpdateProduct(int id)
        {
            ViewBag.ShowBackButton = true;

            var product = await _client.GetFromJsonAsync<UpdateProductDto>($"products/{id}");
            if (product == null)
                return NotFound();

            await LoadBrandsDropdownAsync(product.BrandId);

            ViewBag.ExistingImageUrl = product.ImageUrl;

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto dto, string existingImageUrl)
        {
            var validator = new UpdateProductValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ExistingImageUrl = existingImageUrl;
                await LoadBrandsDropdownAsync(dto.BrandId);
                return View(dto);
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // ✅ Eski görseli sil
                if (!string.IsNullOrWhiteSpace(existingImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, existingImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // ✅ Yeni görseli WebP olarak kaydet
                var folderPath = Path.Combine(_env.WebRootPath, "images", "products");
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
                dto.ImageUrl = "/images/products/" + newFileName;
            }
            else
            {
                // 📌 Yeni resim yoksa mevcut görseli koru
                dto.ImageUrl = existingImageUrl;
            }

            // ✅ API'ye gönderim
            var form = new MultipartFormDataContent
    {
        { new StringContent(dto.Id.ToString()), "Id" },
        { new StringContent(dto.Name ?? ""), "Name" },
        { new StringContent(dto.BrandId.ToString()), "BrandId" },
        { new StringContent(dto.UnitPrice.ToString()), "UnitPrice" },
    };

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                form.Add(new StringContent(dto.ImageUrl), "ImageUrl");

            var response = await _client.PutAsync("products", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Ürün güncellenemedi: {errorMsg}";
            ViewBag.ExistingImageUrl = dto.ImageUrl;
            await LoadBrandsDropdownAsync(dto.BrandId);
            return View(dto);
        }




        [HttpPost]
        public async Task<IActionResult> SoftDeletedProduct(int id)
        {
            var response = await _client.PostAsync($"Products/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        public async Task<IActionResult> UndoSoftDeletedProduct(int id)
        {
            var response = await _client.PostAsync($"Products/undosoftdelete/{id}", null);
            TempData["SuccessMessage"] = response.IsSuccessStatusCode ? "Üst kategori geri alındı." : "Geri alma işlemi başarısız.";
            return RedirectToAction(nameof(DeletedProduct));
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteProduct(int id)
        {
            var response = await _client.DeleteAsync($"products/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(content);
            else
                return BadRequest(content);
        }

        [HttpGet]
        public async Task<IActionResult> ApplyDiscount(int id)
        {
            var dto = await _client.GetFromJsonAsync<UpdateDiscountDto>($"products/discountproduct/{id}");
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyDiscount(UpdateDiscountDto dto)
        {
            var validator = new UpdateDiscountValidator();
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }
            var response = await _client.PostAsJsonAsync("products/applydiscount", dto);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMsg);
                return View(dto);
            }

            TempData["SuccessMessage"] = $"Üründe %{dto.DiscountRate} indirim uygulandı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("products/DeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPost]
        public async Task<IActionResult> UndoSoftDeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("products/UndoSoftDeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteMultiple([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Hiç kategori seçilmedi.");
            var response = await _client.PostAsJsonAsync("products/PermanentDeleteMultiple", ids);

            if (response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Ok(message);
            }

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest(error);
        }

    }
}
