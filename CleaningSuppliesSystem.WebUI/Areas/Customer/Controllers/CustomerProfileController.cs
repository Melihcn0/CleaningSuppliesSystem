using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Customer.CustomerCorporateAddressDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.Customer.CustomerIndivualAddressDto;
using CleaningSuppliesSystem.DTO.DTOs.ValidatorDtos.CustomerProfileValidatorDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleaningSuppliesSystem.WebUI.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    [Area("Customer")]
    public class CustomerProfileController : Controller
    {
        private readonly HttpClient _client;

        public CustomerProfileController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CleaningSuppliesSystemClient");
        }
        private async Task LoadCityDropdownAsync(int? selectedCityId = null, int? selectedDistrictId = null)
        {
            var cities = await _client.GetFromJsonAsync<List<ResultLocationCityDto>>("CustomerAddress/active-city");
            ViewBag.Cities = cities?.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CityName,
                Selected = selectedCityId.HasValue && c.Id == selectedCityId.Value
            }).ToList() ?? new List<SelectListItem>();

            // Eğer şehir seçiliyse, ilçeleri de yükle
            if (selectedCityId.HasValue)
            {
                var districts = await _client.GetFromJsonAsync<List<ResultLocationDistrictDto>>($"CustomerAddress/ByCityIndividual/{selectedCityId}");
                ViewBag.Districts = districts?.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DistrictName,
                    Selected = selectedDistrictId.HasValue && d.Id == selectedDistrictId.Value
                }).ToList() ?? new List<SelectListItem>();
            }
            else
            {
                ViewBag.Districts = new List<SelectListItem>();
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetByCity(int cityId)
        {
            var districts = await _client.GetFromJsonAsync<List<ResultLocationDistrictDto>>($"CustomerAddress/ByCityIndividual/{cityId}");
            return Json(districts ?? new List<ResultLocationDistrictDto>());
        }

        public async Task<IActionResult> Index(string activeTab = "pills-blank")
        {
            await LoadCityDropdownAsync();
            var model = await GetCustomerProfileViewModel();
            if (model == null)
                return NotFound("Kullanıcı bilgileri bulunamadı.");

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? activeTab;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomerProfile(UpdateCustomerProfileDto dto)
        {
            var validator = new UpdateCustomerProfileValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove(error.PropertyName);
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                ViewBag.ActiveTab = "pills-edit-profile";
                var model = await GetCustomerProfileViewModel(updateProfileDto: dto);
                return View("Index", model);
            }

            var response = await _client.PutAsJsonAsync("CustomerProfile", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Müşteri profili başarıyla güncellendi.";
                TempData["ActiveTab"] = "pills-edit-profile";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Müşteri profili güncellenemedi.";
            TempData["ActiveTab"] = "pills-edit-profile";
            var errorModel = await GetCustomerProfileViewModel(updateProfileDto: dto);
            return View("Index", errorModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAddressPartial()
        {
           await LoadCityDropdownAsync();
           ViewBag.AddressType = "Individual";
           var model = new CustomerProfileViewModel
            {
                CreateIndividualAddress = new CreateCustomerIndividualAddressDto(),
                CreateCorporateAddress = new CreateCustomerCorporateAddressDto()
            };
            return PartialView("_CreateAddressPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateIndividualAddressPartial(int id)
        {
            var address = await _client.GetFromJsonAsync<UpdateCustomerIndividualAddressDto>($"CustomerAddress/individualId/{id}");
            if (address == null)
                return NotFound("Adres bulunamadı.");

            // Şehir ve ilçe dropdown'larını yükle
            await LoadCityDropdownAsync(address.CityId, address.DistrictId);

            return PartialView("_UpdateIndividualAddressPartial", address);
        }

        //[HttpGet]
        //public async Task<IActionResult> UpdateCorporateAddressPartial(int id)
        //{
        //    var address = await _client.GetFromJsonAsync<UpdateCustomerCorporateAddressDto>($"CustomerAddress/corporateId/{id}");
        //    if (address == null)
        //        return NotFound("Adres bulunamadı.");
        //    await LoadCityDropdownAsync(address.CityId, address.DistrictId);
        //    return PartialView("_UpdateCorporateAddressPartial", address);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateIndividualAddress(CustomerProfileViewModel model)
        {
            await LoadCityDropdownAsync();
            var dto = model.CreateIndividualAddress;
            var validationResult = await new CreateIndividualAddressValidator().ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove($"CreateIndividualAddress.{error.PropertyName}");
                    ModelState.AddModelError($"CreateIndividualAddress.{error.PropertyName}", error.ErrorMessage);
                }

                await LoadCityDropdownAsync(dto.CityId, dto.DistrictId);
                ViewBag.ActiveTab = "pills-add-address";
                ViewBag.AddressType = "Individual"; // <-- Burayı "Individual" yap

                var vm = await GetCustomerProfileViewModel();
                vm.CreateIndividualAddress = dto;
                return View("Index", vm);
            }

            var response = await _client.PostAsJsonAsync("CustomerAddress/individual", dto);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(content);
                    TempData[json.RootElement.GetProperty("isSuccess").GetBoolean() ? "SuccessMessage" : "ErrorMessage"]
                        = json.RootElement.GetProperty("message").GetString();
                }
                catch { TempData["ErrorMessage"] = "API'den beklenmeyen bir cevap geldi."; }

                TempData["ActiveTab"] = "pills-list-address";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Adres oluşturulamadı.";
            ViewBag.ActiveTab = "pills-add-address";
            ViewBag.AddressType = "Individual"; // <-- Burayı da "Individual" yap

            var failedVm = await GetCustomerProfileViewModel();
            failedVm.CreateIndividualAddress = dto;
            return View("Index", failedVm);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateIndividualAddress(CustomerProfileViewModel model)
        //{
        //    var dto = model.UpdateIndividualAddress;

        //    var validator = new UpdateIndividualAddressValidator();
        //    var validationResult = await validator.ValidateAsync(dto);

        //    if (!validationResult.IsValid)
        //    {
        //        foreach (var error in validationResult.Errors)
        //        {
        //            ModelState.Remove($"UpdateIndividualAddress.{error.PropertyName}");
        //            ModelState.AddModelError($"UpdateIndividualAddress.{error.PropertyName}", error.ErrorMessage);
        //        }
        //        await LoadCityDropdownAsync(dto.CityId, dto.DistrictId);
        //        ViewBag.ActiveTab = "pills-edit-address";
        //        var vm = await GetCustomerProfileViewModel(updateAddressDto: dto);
        //        return View("Index", vm);
        //    }
        //    var response = await _client.PutAsJsonAsync("CustomerAddress/individual", dto);

        //    var content = await response.Content.ReadAsStringAsync();
        //    try
        //    {
        //        var json = System.Text.Json.JsonDocument.Parse(content);
        //        var isSuccess = json.RootElement.GetProperty("isSuccess").GetBoolean();
        //        var message = json.RootElement.GetProperty("message").GetString();

        //        if (isSuccess)
        //            TempData["SuccessMessage"] = message;
        //        else
        //            TempData["ErrorMessage"] = message;
        //    }
        //    catch
        //    {
        //        TempData["ErrorMessage"] = "API’den beklenmeyen bir cevap geldi.";
        //    }

        //    TempData["ActiveTab"] = "pills-list-address";
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCorporateAddress(CustomerProfileViewModel model)
        {
            await LoadCityDropdownAsync();
            var dto = model.CreateCorporateAddress;
            var validationResult = await new CreateCorporateAddressValidator().ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.Remove($"CreateCorporateAddress.{error.PropertyName}");
                    ModelState.AddModelError($"CreateCorporateAddress.{error.PropertyName}", error.ErrorMessage);
                }

                await LoadCityDropdownAsync(dto.CityId, dto.DistrictId);
                ViewBag.ActiveTab = "pills-add-address";
                ViewBag.AddressType = "Corporate";

                var vm = await GetCustomerProfileViewModel();
                vm.CreateCorporateAddress = dto;
                return View("Index", vm);
            }

            var response = await _client.PostAsJsonAsync("CustomerAddress/corporate", dto);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = System.Text.Json.JsonDocument.Parse(content);
                    TempData[json.RootElement.GetProperty("isSuccess").GetBoolean() ? "SuccessMessage" : "ErrorMessage"]
                        = json.RootElement.GetProperty("message").GetString();
                }
                catch
                {
                    TempData["ErrorMessage"] = "API'den beklenmeyen bir cevap geldi.";
                }

                TempData["ActiveTab"] = "pills-list-address";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Adres oluşturulamadı.";
            ViewBag.ActiveTab = "pills-add-address";
            ViewBag.AddressType = "Corporate";

            var failedVm = await GetCustomerProfileViewModel();
            failedVm.CreateCorporateAddress = dto;
            return View("Index", failedVm);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateCorporateAddress(CustomerProfileViewModel model)
        //{
        //    var dto = model.UpdateCorporateAddress;

        //    var validator = new UpdateCorporateAddressValidator();
        //    var validationResult = await validator.ValidateAsync(dto);

        //    if (!validationResult.IsValid)
        //    {
        //        foreach (var error in validationResult.Errors)
        //        {
        //            ModelState.Remove($"UpdateCorporateAddress.{error.PropertyName}");
        //            ModelState.AddModelError($"UpdateCorporateAddress.{error.PropertyName}", error.ErrorMessage);
        //        }
        //        await LoadCityDropdownAsync(dto.CityId, dto.DistrictId);
        //        ViewBag.ActiveTab = "pills-edit-address";
        //        var vm = await GetCustomerProfileViewModel(updateCorporateAddressDto: dto);
        //        return View("Index", vm);
        //    }
        //    var response = await _client.PutAsJsonAsync("CustomerAddress/corporate", dto);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        try
        //        {
        //            var json = System.Text.Json.JsonDocument.Parse(content);
        //            var isSuccess = json.RootElement.GetProperty("isSuccess").GetBoolean();
        //            var message = json.RootElement.GetProperty("message").GetString();

        //            if (isSuccess)
        //                TempData["SuccessMessage"] = message;
        //            else
        //                TempData["ErrorMessage"] = message;
        //        }
        //        catch
        //        {
        //            TempData["ErrorMessage"] = "API’den beklenmeyen bir cevap geldi.";
        //        }

        //        TempData["ActiveTab"] = "pills-list-address";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.ActiveTab = "pills-edit-address";
        //    var failedVm = await GetCustomerProfileViewModel(updateCorporateAddressDto: dto);
        //    TempData["ErrorMessage"] = "Adres güncellenemedi.";
        //    return View("Index", failedVm);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteIndividualAddress(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"customerAddress/permanentIndividual/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    TempData["SuccessMessage"] = content;
                else
                    TempData["ErrorMessage"] = content;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            await LoadCityDropdownAsync();
            var model = await GetCustomerProfileViewModel();

            ViewBag.ActiveTab = "pills-list-address";
            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteCorporateAddress(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"customerAddress/permanentCorporate/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    TempData["SuccessMessage"] = content;
                else
                    TempData["ErrorMessage"] = content;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            await LoadCityDropdownAsync();
            var model = await GetCustomerProfileViewModel();

            ViewBag.ActiveTab = "pills-list-address";
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int addressId, bool newStatus)
        {
            var response = await _client.PostAsync($"customerAddress/togglestatus?addressId={addressId}&newStatus={newStatus}", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Adres durumu güncellenirken bir hata oluştu: {error}";
            }

            TempData["ActiveTab"] = "pills-list-address";
            return RedirectToAction(nameof(Index));
        }

        private async Task<CustomerProfileViewModel> GetCustomerProfileViewModel(
            UpdateCustomerProfileDto? updateProfileDto = null,
            CreateCustomerIndividualAddressDto? createAddressDto = null,
            UpdateCustomerIndividualAddressDto? updateAddressDto = null,
            CreateCustomerCorporateAddressDto? createCorporateAddressDto = null,
            UpdateCustomerCorporateAddressDto? updateCorporateAddressDto = null)
        {
            var profileDto = await _client.GetFromJsonAsync<CustomerProfileDto>("CustomerProfile/Profile");
            if (profileDto == null) return null;

            var updateDto = updateProfileDto
                ?? await _client.GetFromJsonAsync<UpdateCustomerProfileDto>("CustomerProfile/UpdateCustomerProfile");

            var indivivualAddresses = await _client.GetFromJsonAsync<List<CustomerIndividualAddressDto>>($"CustomerAddress/all-individual/{profileDto.Id}");
            var corporateAddresses = await _client.GetFromJsonAsync<List<CustomerCorporateAddressDto>>($"CustomerAddress/all-corporate/{profileDto.Id}");

            return new CustomerProfileViewModel
            {
                CustomerProfile = profileDto,
                UpdateCustomerProfile = updateDto,

                // bireysel adresler
                CustomerIndividualAddresses = indivivualAddresses,
                CreateIndividualAddress = createAddressDto ?? new CreateCustomerIndividualAddressDto(),
                UpdateIndividualAddress = updateAddressDto ?? new UpdateCustomerIndividualAddressDto(),

                // kurumsal adresler
                CustomerCorporateAddresses = corporateAddresses,
                CreateCorporateAddress = createCorporateAddressDto ?? new CreateCustomerCorporateAddressDto(),
                UpdateCorporateAddress = updateCorporateAddressDto ?? new UpdateCustomerCorporateAddressDto(),

            };
        }

    }
}
