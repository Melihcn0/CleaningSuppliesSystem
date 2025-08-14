using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.TopCategoryDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class TopCategoryController : Controller
{
    private readonly HttpClient _client;

    public TopCategoryController(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
    }

    public async Task<IActionResult> Index()
    {
        var topCategories = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("TopCategories/active");
        return View(topCategories);
    }

    public async Task<IActionResult> DeletedTopCategory()
    {
        ViewBag.ShowBackButton = true;
        var deleted = await _client.GetFromJsonAsync<List<ResultTopCategoryDto>>("TopCategories/deleted");
        return View(deleted);
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
            TempData["SuccessMessage"] = "Üst kategori güncellendi.";
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
        TempData["SuccessMessage"] = response.IsSuccessStatusCode ? "Üst kategori geri alındı." : "Geri alma işlemi başarısız.";
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
