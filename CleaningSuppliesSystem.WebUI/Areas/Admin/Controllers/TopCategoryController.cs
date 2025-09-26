using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.TopCategoryDto;
using CleaningSuppliesSystem.WebUI.Helpers;
using CleaningSuppliesSystem.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class TopCategoryController : Controller
{
    private readonly HttpClient _client;
    private readonly PaginationHelper _paginationHelper;

    public TopCategoryController(IHttpClientFactory clientFactory, PaginationHelper paginationHelper)
    {
        _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        _paginationHelper = paginationHelper;
    }
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var response = await _paginationHelper.GetPagedDataAsync<ResultTopCategoryDto>(
            $"TopCategories/active?page={page}&pageSize={pageSize}");

        return View(response);
    }

    public async Task<IActionResult> DeletedTopCategory(int page = 1, int pageSize = 10)
    {
        ViewBag.ShowBackButton = true;
        var response = await _paginationHelper.GetPagedDataAsync<ResultTopCategoryDto>(
            $"TopCategories/deleted?page={page}&pageSize={pageSize}");

        return View(response);
    }

    public IActionResult CreateTopCategory()
    {
        ViewBag.ShowBackButton = true;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTopCategory(CreateTopCategoryDto dto)
    {
        var validator = new CreateTopCategoryValidator();
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

        var response = await _client.PostAsJsonAsync("TopCategories", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Üst kategori başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Üst kategori eklenemedi.";
        return View(dto);
    }

    public async Task<IActionResult> UpdateTopCategory(int id)
    {
        ViewBag.ShowBackButton = true;
        var topCategory = await _client.GetFromJsonAsync<UpdateTopCategoryDto>($"TopCategories/{id}");
        return topCategory == null ? NotFound() : View(topCategory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTopCategory(UpdateTopCategoryDto dto)
    {
        var validator = new UpdateTopCategoryValidator();
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

        var response = await _client.PutAsJsonAsync("TopCategories", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Üst kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ErrorMessage"] = "Üst kategori güncellenemedi.";
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDeletedTopCategory(int id)
    {
        var response = await _client.PostAsync($"TopCategories/softdelete/{id}", null);
        var msg = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return Ok(msg);
        else
            return BadRequest(msg);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UndoSoftDeletedTopCategory(int id)
    {
        var response = await _client.PostAsync($"TopCategories/undosoftdelete/{id}", null);

        if (response.IsSuccessStatusCode)
            TempData["SuccessMessage"] = "Üst kategori çöp kutusundan başarıyla geri alındı.";
        else
            TempData["ErrorMessage"] = "Üst kategorinin çöp kutusundan geri alma işlemi başarısız.";

        return RedirectToAction(nameof(DeletedTopCategory));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PermanentDeleteTopCategory(int id)
    {
        var response = await _client.DeleteAsync($"topCategories/permanent/{id}");
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
        var response = await _client.PostAsJsonAsync("TopCategories/DeleteMultiple", ids);

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
        var response = await _client.PostAsJsonAsync("TopCategories/UndoSoftDeleteMultiple", ids);

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
        var response = await _client.PostAsJsonAsync("TopCategories/PermanentDeleteMultiple", ids);

        if (response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            return Ok(message);
        }

        var error = await response.Content.ReadAsStringAsync();
        return BadRequest(error);
    }

}
