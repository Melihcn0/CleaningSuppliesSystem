using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.RoleDtos;
using CleaningSuppliesSystem.Entity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Roles")]
    [Authorize(Roles = "Developer")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRoleDto> _createRoleValidator;

        public RolesController(IRoleService roleService, IMapper mapper, IValidator<CreateRoleDto> createRoleValidator)
        {
            _roleService = roleService;
            _mapper = mapper;
            _createRoleValidator = createRoleValidator;
        }

        // GET api/roles 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.TGetAllRolesAsync();
            var result = _mapper.Map<List<ResultRoleDto>>(roles);
            return Ok(result);
        }

        // POST api/roles
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto model)
        {
            var validationResult = await _createRoleValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { e.PropertyName, e.ErrorMessage })
                    .ToList();
                return BadRequest(errors);
            }

            var role = _mapper.Map<AppRole>(model);
            var result = await _roleService.TCreateRoleAsync(role);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(errors);
            }

            return Ok(new { Message = "Rol başarıyla eklendi." });
        }


        // GET api/roles/show-create-button
        [HttpGet("show-create-button")]
        public async Task<IActionResult> ShouldShowCreateRoleButton()
        {
            bool showButton = await _roleService.TShouldShowCreateRoleButtonAsync();
            return Ok(showButton);
        }
    }
}
