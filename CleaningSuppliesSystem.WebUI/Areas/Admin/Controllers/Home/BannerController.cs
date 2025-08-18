using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.BannerDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.ToggleStatusValidatorDto;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers.Home
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BannerController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _env;

        public BannerController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _client.GetFromJsonAsync<List<ResultBannerDto>>("banners/active");
            return View(banners);
        }

        public async Task<IActionResult> DeletedBanner()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultBannerDto>>("banners/deleted");
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateBanner()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBanner([FromForm] CreateBannerDto dto)
        {
            var validator = new CreateBannerValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ShowBackButton = true;
                return View(dto);
            }

            // 🌄 Görsel varsa, WebP olarak kaydet
            if (dto.ImageFile != null)
                dto.ImageUrl = await SaveImageAsWebPAsync(dto.ImageFile);

            // 📦 Form veri paketlemesi
            using var form = new MultipartFormDataContent
            {
                { new StringContent(dto.Title ?? ""), "Title" },
                { new StringContent(dto.Subtitle ?? ""), "Subtitle" },
                { new StringContent(dto.Statistic1Title ?? ""), "Statistic1Title" },
                { new StringContent(dto.Statistic1 ?? ""), "Statistic1" },
                { new StringContent(dto.Statistic2Title ?? ""), "Statistic2Title" },
                { new StringContent(dto.Statistic2 ?? ""), "Statistic2" },
                { new StringContent(dto.Statistic3Title ?? ""), "Statistic3Title" },
                { new StringContent(dto.Statistic3 ?? ""), "Statistic3" },
                { new StringContent(dto.Statistic4Title ?? ""), "Statistic4Title" },
                { new StringContent(dto.Statistic4 ?? ""), "Statistic4" },
                { new StringContent(dto.ImageUrl ?? ""), "ImageUrl" }
            };

            var response = await _client.PostAsync("banners", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Banner başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Banner eklenemedi: {errorContent}";
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBanner(int id)
        {
            var dto = await _client.GetFromJsonAsync<UpdateBannerDto>($"banners/{id}");
            if (dto == null) return NotFound();
            ViewBag.ShowBackButton = true;
            ViewBag.ExistingImageUrl = dto.ImageUrl;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBanner(UpdateBannerDto dto, string existingImageUrl)
        {
            var validator = new UpdateBannerValidator();
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
                return View(dto);
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(existingImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, existingImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var folderPath = Path.Combine(_env.WebRootPath, "images", "banners");
                Directory.CreateDirectory(folderPath);

                var newFileName = $"{Guid.NewGuid()}.webp";
                var newFilePath = Path.Combine(folderPath, newFileName);

                using var stream = dto.ImageFile.OpenReadStream();
                using var image = await Image.LoadAsync(stream);

                await image.SaveAsync(newFilePath, new WebpEncoder { Quality = 100 });
                dto.ImageUrl = $"/images/banners/{newFileName}";
            }
            else
            {
                dto.ImageUrl = existingImageUrl;
            }

            using var form = new MultipartFormDataContent
            {
                { new StringContent(dto.Id.ToString()), "Id" },
                { new StringContent(dto.Title ?? ""), "Title" },
                { new StringContent(dto.Title ?? ""), "SubTitle" },
                { new StringContent(dto.Statistic1Title ?? ""), "Statistic1Title" },
                { new StringContent(dto.Statistic1 ?? ""), "Statistic1" },
                { new StringContent(dto.Statistic2Title ?? ""), "Statistic2Title" },
                { new StringContent(dto.Statistic2 ?? ""), "Statistic2" },
                { new StringContent(dto.Statistic3Title ?? ""), "Statistic3Title" },
                { new StringContent(dto.Statistic3 ?? ""), "Statistic3" },
                { new StringContent(dto.Statistic4Title ?? ""), "Statistic4Title" },
                { new StringContent(dto.Statistic4 ?? ""), "Statistic4" },
                { new StringContent(dto.ImageUrl ?? ""), "ImageUrl" }
            };

            var response = await _client.PutAsync("banners", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Banner başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            var errorDetail = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Banner güncellenemedi: {errorDetail}";
            ViewBag.ExistingImageUrl = dto.ImageUrl;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteBanner(int id)
        {
            var response = await _client.PostAsync($"banners/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        public async Task<IActionResult> UndoSoftDeleteBanner(int id)
        {
            var response = await _client.PostAsync($"banners/undosoftdelete/{id}", null);
            TempData["SuccessMessage"] = response.IsSuccessStatusCode ? "Banner geri alındı." : "Geri alma işlemi başarısız.";
            return RedirectToAction(nameof(DeletedBanner));
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteBanner(int id)
        {
            // Banner bilgilerini al
            var bannerResponse = await _client.GetAsync($"banners/{id}");
            if (!bannerResponse.IsSuccessStatusCode)
                return BadRequest("Banner bilgisi alınamadı.");

            var bannerJson = await bannerResponse.Content.ReadAsStringAsync();
            var banner = JsonConvert.DeserializeObject<ResultBannerDto>(bannerJson);

            // Kalıcı silme isteği
            var response = await _client.DeleteAsync($"banners/permanent/{id}");
            var content = await response.Content.ReadAsStringAsync();

            // API başarısızsa cevapla
            if (!response.IsSuccessStatusCode)
                return BadRequest(content);

            // Görsel varsa fiziksel olarak sil
            if (!string.IsNullOrWhiteSpace(banner.ImageUrl))
            {
                var relativePath = banner.ImageUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
                var imagePath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            return Ok(content);
        }

        private async Task<string> SaveImageAsWebPAsync(IFormFile imageFile)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images", "banners");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + ".webp";
            var filePath = Path.Combine(uploads, fileName);

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            ms.Position = 0;

            using Image<Rgba32> image = Image.Load<Rgba32>(ms);

            var encoder = new WebpEncoder { Quality = 100 };

            await image.SaveAsync(filePath, encoder);
            return "/images/banners/" + fileName;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int bannerId, bool newStatus)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"banners/togglestatus?bannerId={bannerId}&newStatus={newStatus}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Banner durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

    }
}
