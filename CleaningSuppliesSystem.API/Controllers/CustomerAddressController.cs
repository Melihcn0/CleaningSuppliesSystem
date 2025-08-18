using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerIndivivualDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Customer")]
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerInvdivualAddressService _customerAddressService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public CustomerAddressController(ICustomerInvdivualAddressService customerAddressService, IMapper mapper, IUserService userService)
        {
            _customerAddressService = customerAddressService;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> Get(int userId)
        {
            var customerAddresses = await _customerAddressService.TGetAllAddressesAsync(userId);
            return Ok(customerAddresses);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customerAddress = await _customerAddressService.TGetAddressByIdAsync(id);
            if (customerAddress == null)
                return NotFound("Müşteri adresi bulunamadı.");

            return Ok(customerAddress);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerIndivivualAddressDto createCustomerAddressDto)
        {
            // Kullanıcıyı al
            var userIdentifier = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdentifier))
                return Ok(new { isSuccess = false, message = "Kullanıcı oturumu bulunamadı." });

            var user = await _userService.GetUserByEmailAsync(userIdentifier)
                       ?? await _userService.GetUserByIdAsync(int.Parse(userIdentifier));

            if (user == null)
                return Ok(new { isSuccess = false, message = "Kullanıcı bulunamadı." });

            createCustomerAddressDto.AppUserId = user.Id;

            var (isSuccess, message, createdId) = await _customerAddressService.TCreateCustomerAddressAsync(createCustomerAddressDto);

            if (!isSuccess)
                return Ok(new { isSuccess = false, message }); // Burada artık BadRequest yerine OK dönüyoruz

            return Ok(new { isSuccess = true, message = "Müşteri adresi başarıyla oluşturuldu.", id = createdId });
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerIndivivualAddressDto updateCustomerAddressDto)
        {
            var (isSuccess, message, updatedId) = await _customerAddressService.TUpdateCustomerAddressAsync(updateCustomerAddressDto);

            if (!isSuccess)
                return BadRequest(new { message });

            return Ok(new { message = "Müşteri adresi başarıyla güncellendi.", id = updatedId });
        }

        [HttpPost("togglestatus")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int addressId, [FromQuery] bool newStatus)
        {
            var result = await _customerAddressService.ToggleCustomerAddressStatusAsync(addressId, newStatus);
            if (!result)
                return BadRequest("Adres durumu güncellenemedi.");

            return Ok("Adres durumu güncellendi.");
        }


    }
}
