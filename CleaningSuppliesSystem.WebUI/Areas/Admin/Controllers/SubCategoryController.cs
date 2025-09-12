using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.CategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ProductDtos;
using CleaningSuppliesSystem.DTO.DTOs.SubCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.SubCategoryDto;
using CleaningSuppliesSystem.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class SubCategoryController : Controller
{
    private readonly HttpClient _client;

    public SubCategoryController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
    }
    private async Task LoadTopCategoriesDropdown()
    {
        var topCategories = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("topCategories/active");
        ViewBag.TopCategories = topCategories.Select(tc => new SelectListItem
        {
            Text = tc.Name,
            Value = tc.Id.ToString()
        }).ToList();
    }

    [HttpGet]
    public async Task<IActionResult> GetSubCategoriesByTopCategory(int topCategoryId)
    {
        var subCategories = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>($"subCategories/ByTopCategory/{topCategoryId}");
        if (subCategories == null || !subCategories.Any())
            return Content("<option value=''>Alt kategori bulunamadı</option>", "text/html");

        var options = new List<string>
            {
                "<option value='' selected disabled>Alt kategori seçin</option>"
            };

        options.AddRange(subCategories.Select(sc => $"<option value='{sc.Id}'>{sc.Name}</option>"));
        return Content(string.Join("", options), "text/html");
    }

    public async Task<IActionResult> Index()
    {
        var resultDtos = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>("subCategories/active");

        var vm = new SubCategoryViewModel
        {
            SubCategoryViewList = resultDtos
        };

        return View(vm);
    }

    public async Task<IActionResult> DeletedSubCategory()
    {
        ViewBag.ShowBackButton = true;
        var resultdeletedDtos = await _client.GetFromJsonAsync<List<ResultSubCategoryDto>>("subCategories/deleted");
        var vm = new SubCategoryViewModel
        {
            SubCategoryViewList = resultdeletedDtos
        };

        return View(vm);
    }

    public async Task<IActionResult> CreateSubCategory()
    {
        ViewBag.ShowBackButton = true;
        await LoadTopCategoriesDropdown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSubCategory(CreateSubCategoryDto dto)
    {
        await LoadTopCategoriesDropdown();
        var validator = new CreateSubCategoryValidator();
        var validation = await validator.ValidateAsync(dto);

        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.Remove(error.PropertyName);
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(dto);
        }

        var response = await _client.PostAsJsonAsync("SubCategories", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Alt kategori başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Alt kategori eklenemedi.";
        return View(dto);
    }

    public async Task<IActionResult> UpdateSubCategory(int id)
    {
        await LoadTopCategoriesDropdown();
        ViewBag.ShowBackButton = true;
        var subCategory = await _client.GetFromJsonAsync<UpdateSubCategoryDto>($"SubCategories/{id}");
        return subCategory == null ? NotFound() : View(subCategory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSubCategory(UpdateSubCategoryDto dto)
    {
        await LoadTopCategoriesDropdown();
        var validator = new UpdateSubCategoryValidator();
        var validation = await validator.ValidateAsync(dto);

        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.Remove(error.PropertyName);
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(dto);
        }

        var response = await _client.PutAsJsonAsync("SubCategories", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Alt kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Alt kategori güncellenemedi.";
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeletedSubCategory(int id)
    {
        var response = await _client.PostAsync($"SubCategories/softdelete/{id}", null);
        var msg = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return Ok(msg);
        else
            return BadRequest(msg);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UndoSoftDeletedSubCategory(int id)
    {
        var response = await _client.PostAsync($"subCategories/undosoftdelete/{id}", null);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Alt kategori başarıyla çöp kutusundan geri alındı.";
        else
            TempData["ErrorMessage"] = "Alt kategorinin çöp kutusundan geri alma işlemi başarısız.";

        return RedirectToAction(nameof(DeletedSubCategory));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PermanentDeleteSubCategory(int id)
    {
        var response = await _client.DeleteAsync($"SubCategories/permanent/{id}");
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return Ok(content);
        else
            return BadRequest(content);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("Hiç kategori seçilmedi.");
        var response = await _client.PostAsJsonAsync("SubCategories/DeleteMultiple", ids);

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
        var response = await _client.PostAsJsonAsync("SubCategories/UndoSoftDeleteMultiple", ids);

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
        var response = await _client.PostAsJsonAsync("SubCategories/PermanentDeleteMultiple", ids);

        if (response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            return Ok(message);
        }

        var error = await response.Content.ReadAsStringAsync();
        return BadRequest(error);
    }


}
