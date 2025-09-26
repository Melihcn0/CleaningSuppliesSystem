using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerCorporateDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using CleaningSuppliesSystem.DTO.DTOs.LocationDtos;
using CleaningSuppliesSystem.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Customer")]
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerIndividualAddressService _customerIndividualAddressService;
        private readonly ICustomerCorporateAddressService _customerCorporateAddressService;
        private readonly ILocationCityService _locationCityService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public CustomerAddressController(ICustomerIndividualAddressService customerIndividualAddressService, ICustomerCorporateAddressService customerCorporateAddressService, ILocationCityService locationCityService, IMapper mapper, IUserService userService)
        {
            _customerIndividualAddressService = customerIndividualAddressService;
            _customerCorporateAddressService = customerCorporateAddressService;
            _locationCityService = locationCityService;
            _mapper = mapper;
            _userService = userService;
        }
        [HttpGet("active-city")]
        public async Task<IActionResult> GetActiveCity()
        {
            var result = await _locationCityService.TGetActiveLocationCitysAsync();
            return Ok(result);
        }
        [HttpGet("ByCityIndividual/{cityId}")]
        public async Task<IActionResult> GetByCityIndividual(int cityId)
        {
            var cities = await _customerIndividualAddressService.TGetActiveByCityIdAsync(cityId);
            return Ok(cities);
        }

        [HttpGet("all-individual/{userId}")]
        public async Task<IActionResult> GetAllIndivivual(int userId)
        {
            var customerAddresses = await _customerIndividualAddressService.TGetAllAddressesAsync(userId);
            return Ok(customerAddresses);
        }
        [HttpGet("individualId/{id}")]
        public async Task<IActionResult> GetIndivivualById(int id)
        {
            var customerAddress = await _customerIndividualAddressService.TGetAddressByIdAsync(id);
            if (customerAddress == null)
                return NotFound("Müşteri bireysel adresi bulunamadı.");

            return Ok(customerAddress);
        }
        [HttpGet("all-corporate/{userId}")]
        public async Task<IActionResult> GetAllCorporate(int userId)
        {
            var customerAddresses = await _customerCorporateAddressService.TGetAllAddressesAsync(userId);
            return Ok(customerAddresses);
        }
        [HttpGet("corporateId/{id}")]
        public async Task<IActionResult> GetCorporateById(int id)
        {
            var customerAddress = await _customerCorporateAddressService.TGetAddressByIdAsync(id);
            if (customerAddress == null)
                return NotFound("Müşteri kurumsal adresi bulunamadı.");

            return Ok(customerAddress);
        }

        [HttpPost("individual")]
        public async Task<IActionResult> CreateIndividual([FromBody] CreateCustomerIndividualAddressDto dto)
        {
            var userIdentifier = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdentifier))
                return Ok(new { isSuccess = false, message = "Kullanıcı oturumu bulunamadı." });

            var user = await _userService.TGetUserByEmailAsync(userIdentifier)
                       ?? await _userService.TGetUserByIdAsync(int.Parse(userIdentifier));

            if (user == null)
                return Ok(new { isSuccess = false, message = "Kullanıcı bulunamadı." });

            dto.AppUserId = user.Id;

            var (isSuccess, message, createdId) =
            await _customerIndividualAddressService.TCreateCustomerIndividualAddressAsync(dto);


            if (!isSuccess)
                return Ok(new { isSuccess = false, message });

            return Ok(new { isSuccess = true, message = "Müşteri adresi başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPost("corporate")]
        public async Task<IActionResult> CreateCorporate([FromBody] CreateCustomerCorporateAddressDto dto)
        {
            var userIdentifier = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdentifier))
                return Ok(new { isSuccess = false, message = "Kullanıcı oturumu bulunamadı." });

            var user = await _userService.TGetUserByEmailAsync(userIdentifier)
                       ?? await _userService.TGetUserByIdAsync(int.Parse(userIdentifier));

            if (user == null)
                return Ok(new { isSuccess = false, message = "Kullanıcı bulunamadı." });

            dto.AppUserId = user.Id;

            var (isSuccess, message, createdId) =
            await _customerCorporateAddressService.TCreateCustomerCorporateAddressAsync(dto);

            if (!isSuccess)
                return Ok(new { isSuccess = false, message });

            return Ok(new { isSuccess = true, message = "Müşteri adresi başarıyla eklendi.", id = createdId });
        }

        [HttpPut("individual")]
        public async Task<IActionResult> UpdateIndividual([FromBody] UpdateCustomerIndividualAddressDto dto)
        {
            var (isSuccess, message, updatedId) = await _customerIndividualAddressService
                .TUpdateCustomerIndividualAddressAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message, id = updatedId });
        }

        [HttpPut("corporate")]
        public async Task<IActionResult> UpdateCorporate([FromBody] UpdateCustomerCorporateAddressDto dto)
        {
            var (isSuccess, message, updatedId) = await _customerCorporateAddressService
                .TUpdateCustomerCorporateAddressAsync(dto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message, id = updatedId });
        }

        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int addressId, [FromQuery] bool newStatus)
        {
            var result = await _customerIndividualAddressService
                .TToggleCustomerAddressStatusAsync(addressId, newStatus);

            if (!result)
                return BadRequest("Adres durumu güncellenemedi.");

            return Ok(new { isSuccess = true, message = "Adres durumu güncellendi." });
        }

        [HttpDelete("permanentIndividual/{id}")]
        public async Task<IActionResult> PermanentDeleteIndividual(int id)
        {
            var address = await _customerIndividualAddressService.TGetAddressByIdAsync(id);
            if (address == null)
                return NotFound("Bireysel adres bulunamadı.");

            var result = await _customerIndividualAddressService.TPermanentDeleteCustomerIndividualAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("permanentCorporate/{id}")]
        public async Task<IActionResult> PermanentDeleteCorporate(int id)
        {
            var address = await _customerCorporateAddressService.TGetAddressByIdAsync(id);
            if (address == null)
                return NotFound("Kurumsal adres bulunamadı.");

            var result = await _customerCorporateAddressService.TPermanentDeleteCustomerCorporateAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

    }
}
