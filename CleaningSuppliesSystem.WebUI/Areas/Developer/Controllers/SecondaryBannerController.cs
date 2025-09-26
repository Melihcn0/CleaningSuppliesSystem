using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using CleaningSuppliesSystem.DTO.DTOs.Home.SecondaryBannerDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Home.SecondaryBannerDto;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.ImageSharp.Size;
using System.Text;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Controllers.Home
{
    [Area("Developer")]
    [Authorize(Roles = "Developer")]
    public class SecondaryBannerController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _env;
        private readonly PaginationHelper _paginationHelper;

        public SecondaryBannerController(IHttpClientFactory clientFactory, IWebHostEnvironment env, PaginationHelper paginationHelper)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _env = env;
            _paginationHelper = paginationHelper;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var response = await _paginationHelper.GetPagedDataAsync<ResultSecondaryBannerDto>(
                $"developerSecondaryBanners/active?page={page}&pageSize={pageSize}");

            return View(response);
        }

        public async Task<IActionResult> DeletedSecondaryBanner(int page = 1, int pageSize = 10)
        {
            ViewBag.ShowBackButton = true;
            var response = await _paginationHelper.GetPagedDataAsync<ResultSecondaryBannerDto>(
                $"developerSecondaryBanners/deleted?page={page}&pageSize={pageSize}");

            return View(response);
        }

        [HttpGet]
        public IActionResult CreateSecondaryBanner()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSecondaryBanner([FromForm] CreateSecondaryBannerDto dto)
        {
            try
            {
                // ✅ Validasyon
                var validator = new CreateSecondaryBannerValidator();
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

                // ✅ Dosyaları WebP olarak kaydet
                if (dto.ImageFile1 != null)
                    dto.ImageUrl1 = await SaveImageAsWebPAsync(dto.ImageFile1);
                if (dto.ImageFile2 != null)
                    dto.ImageUrl2 = await SaveImageAsWebPAsync(dto.ImageFile2);
                if (dto.ImageFile3 != null)
                    dto.ImageUrl3 = await SaveImageAsWebPAsync(dto.ImageFile3);

                // ✅ Form veri paketlemesi (sadece URL gönderiliyor)
                using var form = new MultipartFormDataContent
                {
                    { new StringContent(dto.Title1 ?? ""), "Title1" },
                    { new StringContent(dto.Title2 ?? ""), "Title2" },
                    { new StringContent(dto.Title3 ?? ""), "Title3" },
                    { new StringContent(dto.Description1 ?? ""), "Description1" },
                    { new StringContent(dto.Description2 ?? ""), "Description2" },
                    { new StringContent(dto.Description3 ?? ""), "Description3" },
                    { new StringContent(dto.ButtonTitle1 ?? ""), "ButtonTitle1" },
                    { new StringContent(dto.ButtonTitle2 ?? ""), "ButtonTitle2" },
                    { new StringContent(dto.ButtonTitle3 ?? ""), "ButtonTitle3" },
                    { new StringContent(dto.ImageUrl1 ?? ""), "ImageUrl1" },
                    { new StringContent(dto.ImageUrl2 ?? ""), "ImageUrl2" },
                    { new StringContent(dto.ImageUrl3 ?? ""), "ImageUrl3" }
                };

                // ✅ API çağrısı
                var response = await _client.PostAsync("developerSecondaryBanners", form);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "İkincil banner başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Detaylı hatayı göster
                    TempData["ErrorMessage"] = $"İkincil banner eklenemedi. Status: {response.StatusCode}, İçerik: {responseContent}";
                    ViewBag.ShowBackButton = true;
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                // Tüm beklenmeyen hataları yakala ve detaylı göster
                TempData["ErrorMessage"] = $"Beklenmeyen bir hata oluştu: {ex.Message}\n{ex.StackTrace}";
                ViewBag.ShowBackButton = true;
                return View(dto);
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateSecondaryBanner(int id)
        {
            var dto = await _client.GetFromJsonAsync<UpdateSecondaryBannerDto>($"developerSecondaryBanners/{id}");

            // BREAKPOINT BURAYA KOY
            // dto içindeki Title1, Title2, Title3, Description1 vb. dolu mu bak
            if (dto == null) return NotFound();

            ViewBag.ShowBackButton = true;
            ViewBag.ExistingImageUrl1 = dto.ImageUrl1;
            ViewBag.ExistingImageUrl2 = dto.ImageUrl2;
            ViewBag.ExistingImageUrl3 = dto.ImageUrl3;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSecondaryBanner(UpdateSecondaryBannerDto dto)
        {
            // ✅ Validasyon
            var validator = new UpdateSecondaryBannerValidator();
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

            // ✅ Resimleri WebP olarak kaydet
            if (dto.ImageFile1 is { Length: > 0 })
            {
                DeleteImageFileIfExists(dto.ImageUrl1);
                dto.ImageUrl1 = await SaveImageAsWebPAsync(dto.ImageFile1);
            }
            if (dto.ImageFile2 is { Length: > 0 })
            {
                DeleteImageFileIfExists(dto.ImageUrl2);
                dto.ImageUrl2 = await SaveImageAsWebPAsync(dto.ImageFile2);
            }
            if (dto.ImageFile3 is { Length: > 0 })
            {
                DeleteImageFileIfExists(dto.ImageUrl3);
                dto.ImageUrl3 = await SaveImageAsWebPAsync(dto.ImageFile3);
            }

            // ✅ Multipart form-data içerik
            using var formData = new MultipartFormDataContent();

            // Text alanları
            formData.Add(new StringContent(dto.Id.ToString()), "Id");
            formData.Add(new StringContent(dto.Title1 ?? ""), "Title1");
            formData.Add(new StringContent(dto.Description1 ?? ""), "Description1");
            formData.Add(new StringContent(dto.ButtonTitle1 ?? ""), "ButtonTitle1");

            formData.Add(new StringContent(dto.Title2 ?? ""), "Title2");
            formData.Add(new StringContent(dto.Description2 ?? ""), "Description2");
            formData.Add(new StringContent(dto.ButtonTitle2 ?? ""), "ButtonTitle2");

            formData.Add(new StringContent(dto.Title3 ?? ""), "Title3");
            formData.Add(new StringContent(dto.Description3 ?? ""), "Description3");
            formData.Add(new StringContent(dto.ButtonTitle3 ?? ""), "ButtonTitle3");

            // Resim URL’leri
            formData.Add(new StringContent(dto.ImageUrl1 ?? ""), "ImageUrl1");
            formData.Add(new StringContent(dto.ImageUrl2 ?? ""), "ImageUrl2");
            formData.Add(new StringContent(dto.ImageUrl3 ?? ""), "ImageUrl3");

            // Dosya alanları
            if (dto.ImageFile1 != null && dto.ImageFile1.Length > 0)
            {
                var streamContent = new StreamContent(dto.ImageFile1.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.ImageFile1.ContentType);
                formData.Add(streamContent, "ImageFile1", dto.ImageFile1.FileName);
            }

            if (dto.ImageFile2 != null && dto.ImageFile2.Length > 0)
            {
                var streamContent = new StreamContent(dto.ImageFile2.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.ImageFile2.ContentType);
                formData.Add(streamContent, "ImageFile2", dto.ImageFile2.FileName);
            }

            if (dto.ImageFile3 != null && dto.ImageFile3.Length > 0)
            {
                var streamContent = new StreamContent(dto.ImageFile3.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.ImageFile3.ContentType);
                formData.Add(streamContent, "ImageFile3", dto.ImageFile3.FileName);
            }

            // ✅ API çağrısı
            var response = await _client.PutAsync("developerSecondaryBanners", formData);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "İkincil banner başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            var errorDetail = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"İkincil banner güncellenemedi: {errorDetail}";
            return View(dto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteSecondaryBanner(int id)
        {
            var response = await _client.PostAsync($"developerSecondaryBanners/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoSoftDeleteSecondaryBanner(int id)
        {
            var response = await _client.PostAsync($"developerSecondaryBanners/undosoftdelete/{id}", null);
            TempData["SuccessMessage"] = response.IsSuccessStatusCode ? "İkincil banner geri alındı." : "Geri alma işlemi başarısız.";
            return RedirectToAction(nameof(DeletedSecondaryBanner));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteSecondaryBanner(int id)
        {
            var responseBanner = await _client.GetAsync($"developerSecondaryBanners/{id}");
            if (!responseBanner.IsSuccessStatusCode)
                return BadRequest("İkincil banner bilgisi alınamadı.");

            var bannerDto = JsonConvert.DeserializeObject<ResultSecondaryBannerDto>(
                await responseBanner.Content.ReadAsStringAsync());

            var deleteResponse = await _client.DeleteAsync($"developerSecondaryBanners/permanent/{id}");
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

            if (!deleteResponse.IsSuccessStatusCode)
                return BadRequest(deleteContent);

            DeleteImageFileIfExists(bannerDto.ImageUrl1);
            DeleteImageFileIfExists(bannerDto.ImageUrl2);
            DeleteImageFileIfExists(bannerDto.ImageUrl3);

            return Ok(deleteContent);
        }
        // ✅ Dosya silme metodu
        private void DeleteImageFileIfExists(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            var relativePath = imageUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (IOException ex)
                {
                    // Dosya başka bir process tarafından kullanılıyor olabilir
                    // Burada loglayabilirsin ama crash olmasına izin verme
                    Console.WriteLine($"Dosya silinirken hata: {ex.Message}");
                }
            }
        }
        // ✅ Resmi WebP olarak kaydet
        private async Task<string> SaveImageAsWebPAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Geçersiz resim dosyası.");

            var uploads = Path.Combine(_env.WebRootPath, "images", "secondaryBanners");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + ".webp";
            var filePath = Path.Combine(uploads, fileName);

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            ms.Position = 0;

            using var image = await Image.LoadAsync(ms); // otomatik format algılar
            var encoder = new WebpEncoder { Quality = 100 };
            await image.SaveAsync(filePath, encoder);

            return "/images/secondaryBanners/" + fileName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int secondaryBannerId, bool newStatus)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"developerSecondaryBanners/togglestatus?secondaryBannerId={secondaryBannerId}&newStatus={newStatus}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "İkincil banner durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }


    }
}
