using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
using CleaningSuppliesSystem.DTO.DTOs.TopCategoryDtos;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Finances")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FinancesController : ControllerBase
    {
        private readonly IFinanceService _financeService;
        private readonly IMapper _mapper;

        public FinancesController(
            IFinanceService financeService,
            IMapper mapper)
        {
            _financeService = financeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = await _financeService.TGetListAsync();
            var result = _mapper.Map<List<ResultFinanceDto>>(values);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveFinances(int page = 1, int pageSize = 10)
        {
            var finances = await _financeService.TGetActiveFinancesAsync();
            var totalCount = finances.Count;

            var pagedData = finances
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultFinanceDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("active-all")]
        public async Task<IActionResult> GetActiveAllFinances()
        {
            var finances = await _financeService.TGetActiveFinancesAsync();
            var result = _mapper.Map<List<ResultTopCategoryDto>>(finances);
            return Ok(result);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedFinances(int page = 1, int pageSize = 10)
        {
            var finances = await _financeService.TGetDeletedFinancesAsync();
            var totalCount = finances.Count;

            var pagedData = finances
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResponse<ResultFinanceDto>
            {
                Data = pagedData,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _financeService.TGetByIdAsync(id);
            if (value == null)
                return NotFound("Finans kaydı bulunamadı");

            var result = _mapper.Map<ResultFinanceDto>(value);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFinanceDto createFinanceDto)
        {
            var (isSuccess, message, createdId) = await _financeService.TCreateFinanceAsync(createFinanceDto);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Finans kaydı başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateFinanceDto updateFinanceDto)
        {
            var (isSuccess, message, updatedId) = await _financeService.TUpdateFinanceAsync(updateFinanceDto);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Finans kaydı başarıyla oluşturuldu.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _financeService.TSoftDeleteFinanceAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var result = await _financeService.TUndoSoftDeleteFinanceAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        // Permanent Delete
        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var entity = await _financeService.TGetByIdAsync(id);
            if (entity == null)
                return NotFound("Finans kaydı bulunamadı");

            if (!entity.IsDeleted)
                return BadRequest("Finans kaydı silinmiş değil. Önce silmeniz gerekir.");

            await _financeService.TDeleteAsync(id);
            return Ok("Finans kaydı çöp kutusundan kalıcı olarak silindi.");
        }

        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek finans kaydı bulunamadı.");

            var results = await _financeService.TSoftDeleteRangeFinanceAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili finans kayıtları başarıyla çöp kutusuna taşındı.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak finans kaydı bulunamadı.");

            var results = await _financeService.TUndoSoftDeleteRangeFinanceAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili finans kaydı başarıyla geri alındı.");
        }
        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek finans kaydı bulunamadı.");

            await _financeService.TPermanentDeleteRangeFinanceAsync(ids);
            return Ok("Seçili finans kayıtları çöp kutusundan kalıcı olarak silindi.");
        }
    }
}
