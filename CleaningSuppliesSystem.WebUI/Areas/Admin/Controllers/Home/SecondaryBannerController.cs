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
using CleaningSuppliesSystem.DTO.DTOs.Home.BannerDtos;
using System.Text;

namespace CleaningSuppliesSystem.WebUI.Areas.Admin.Controllers.Home
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SecondaryBannerController : Controller
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _env;

        public SecondaryBannerController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var secondaryBanners = await _client.GetFromJsonAsync<List<ResultSecondaryBannerDto>>("SecondaryBanners/active");
            return View(secondaryBanners);
        }

        public async Task<IActionResult> DeletedSecondaryBanner()
        {
            ViewBag.ShowBackButton = true;
            var values = await _client.GetFromJsonAsync<List<ResultSecondaryBannerDto>>("SecondaryBanners/deleted");
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateSecondaryBanner()
        {
            ViewBag.ShowBackButton = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSecondaryBanner([FromForm] CreateSecondaryBannerDto dto)
        {
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

            if (dto.ImageFile1 != null)
                dto.ImageUrl1 = await SaveImageAsWebPAsync(dto.ImageFile1);
            if (dto.ImageFile2 != null)
                dto.ImageUrl2 = await SaveImageAsWebPAsync(dto.ImageFile2);
            if (dto.ImageFile3 != null)
                dto.ImageUrl3 = await SaveImageAsWebPAsync(dto.ImageFile3);

            // 📦 Form veri paketlemesi
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

            var response = await _client.PostAsync("SecondaryBanners", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "İkincil banner başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"İkincil banner eklenemedi: {errorContent}";
            ViewBag.ShowBackButton = true;
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSecondaryBanner(int id)
        {
            var dto = await _client.GetFromJsonAsync<UpdateSecondaryBannerDto>($"SecondaryBanners/{id}");
            if (dto == null) return NotFound();
            ViewBag.ShowBackButton = true;
            ViewBag.ExistingImageUrl1 = dto.ImageUrl1;
            ViewBag.ExistingImageUrl2 = dto.ImageUrl2;
            ViewBag.ExistingImageUrl3 = dto.ImageUrl3;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSecondaryBanner(UpdateSecondaryBannerDto dto)
        {
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
                ViewBag.ExistingImageUrl1 = dto.ImageUrl1;
                ViewBag.ExistingImageUrl2 = dto.ImageUrl2;
                ViewBag.ExistingImageUrl3 = dto.ImageUrl3;
                return View(dto);
            }

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

            using var form = new MultipartFormDataContent
            {
                { new StringContent(dto.Id.ToString()), "Id" },
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

            var response = await _client.PutAsync("SecondaryBanners", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "İkincil banner başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            var errorDetail = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"İkincil banner güncellenemedi: {errorDetail}";

            ViewBag.ExistingImageUrl1 = dto.ImageUrl1;
            ViewBag.ExistingImageUrl2 = dto.ImageUrl2;
            ViewBag.ExistingImageUrl3 = dto.ImageUrl3;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SoftDeleteSecondaryBanner(int id)
        {
            var response = await _client.PostAsync($"SecondaryBanners/softdelete/{id}", null);
            var msg = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return Ok(msg);
            else
                return BadRequest(msg);
        }

        [HttpPost]
        public async Task<IActionResult> UndoSoftDeleteSecondaryBanner(int id)
        {
            var response = await _client.PostAsync($"SecondaryBanners/undosoftdelete/{id}", null);
            TempData["SuccessMessage"] = response.IsSuccessStatusCode ? "İkincil banner geri alındı." : "Geri alma işlemi başarısız.";
            return RedirectToAction(nameof(DeletedSecondaryBanner));
        }

        [HttpPost]
        public async Task<IActionResult> PermanentDeleteSecondaryBanner(int id)
        {
            var responseBanner = await _client.GetAsync($"SecondaryBanners/{id}");
            if (!responseBanner.IsSuccessStatusCode)
                return BadRequest("İkincil banner bilgisi alınamadı.");

            var bannerDto = JsonConvert.DeserializeObject<ResultSecondaryBannerDto>(
                await responseBanner.Content.ReadAsStringAsync());

            var deleteResponse = await _client.DeleteAsync($"SecondaryBanners/permanent/{id}");
            var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

            if (!deleteResponse.IsSuccessStatusCode)
                return BadRequest(deleteContent);

            DeleteImageFileIfExists(bannerDto.ImageUrl1);
            DeleteImageFileIfExists(bannerDto.ImageUrl2);
            DeleteImageFileIfExists(bannerDto.ImageUrl3);

            return Ok(deleteContent);
        }
        private void DeleteImageFileIfExists(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            var relativePath = imageUrl.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
        private async Task<string> SaveImageAsWebPAsync(IFormFile imageFile)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images", "secondaryBanners");
            Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + ".webp";
            var filePath = Path.Combine(uploads, fileName);

            using var ms = new MemoryStream();
            await imageFile.CopyToAsync(ms);
            ms.Position = 0;

            using Image<Rgba32> image = Image.Load<Rgba32>(ms);

            var encoder = new WebpEncoder { Quality = 100 };

            await image.SaveAsync(filePath, encoder);
            return "/images/SecondaryBanners/" + fileName;
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int secondaryBannerId, bool newStatus)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"secondaryBanners/togglestatus?secondaryBannerId={secondaryBannerId}&newStatus={newStatus}", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "İkincil banner durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }


    }
}
