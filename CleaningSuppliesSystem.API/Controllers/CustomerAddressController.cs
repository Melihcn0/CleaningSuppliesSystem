using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.BrandDtos;
using CleaningSuppliesSystem.DTO.DTOs.Customer.CustomerProfileDtos;
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
        private readonly ICustomerAddressService _customerAddressService;
        private readonly IMapper _mapper;
        public CustomerAddressController(ICustomerAddressService customerAddressService, IMapper mapper)
        {
            _customerAddressService = customerAddressService;
            _mapper = mapper;
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
        public async Task<IActionResult> Create([FromBody] CreateCustomerAddressDto createCustomerAddressDto)
        {
            var (isSuccess, message, createdId) = await _customerAddressService.TCreateCustomerAddressAsync(createCustomerAddressDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Müşteri adresi başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerAddressDto updateCustomerAddressDto)
        {
            var (isSuccess, message, updatedId) = await _customerAddressService.TUpdateCustomerAddressAsync(updateCustomerAddressDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Müşteri adresi başarıyla güncellendi.", id = updatedId });
        }
    }
}
