using AutoMapper;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.FinanceDtos;
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
        public async Task<IActionResult> GetActiveFinances()
        {
            var result = await _financeService.TGetActiveFinancesAsync();
            return Ok(result);
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedFinances()
        {
            var result = await _financeService.TGetDeletedFinancesAsync();
            return Ok(result);
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
            return Ok(new { message = "Finans Kaydı başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateFinanceDto updateFinanceDto)
        {
            var (isSuccess, message, updatedId) = await _financeService.TUpdateFinanceAsync(updateFinanceDto);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }
            return Ok(new { message = "Finans Kaydı başarıyla oluşturuldu.", id = updatedId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var (IsSuccess, Message, _UndoSoftDeleteId) = await _financeService.TSoftDeleteFinanceAsync(id);
            if (!IsSuccess)
                return BadRequest(Message);
            return Ok(Message);
        }

        [HttpPost("undosoftdelete/{id}")]
        public async Task<IActionResult> UndoSoftDelete(int id)
        {
            var (IsSuccess, Message, UndoSoftDeleteId) = await _financeService.TUndoSoftDeleteFinanceAsync(id);
            if (!IsSuccess)
                return BadRequest(Message);
            return Ok(Message);
        }

        // Permanent Delete
        [HttpDelete("permanent/{id}")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var entity = await _financeService.TGetByIdAsync(id);
            if (entity == null)
                return NotFound("Finans kaydı bulunamadı");

            if (!entity.IsDeleted)
                return BadRequest("Finans kaydı soft silinmiş değil. Önce soft silmeniz gerekir.");

            await _financeService.TDeleteAsync(id);

            return Ok("Finans kaydı kalıcı olarak silindi");
        }

        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek finans bulunamadı.");

            var results = await _financeService.TSoftDeleteRangeFinanceAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Tüm finanslar başarıyla silindi.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak finans bulunamadı.");

            var results = await _financeService.TUndoSoftDeleteRangeFinanceAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Tüm finanslar başarıyla geri alındı.");
        }
        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek finans bulunamadı.");

            await _financeService.TPermanentDeleteRangeFinanceAsync(ids);
            return Ok("Finanslar başarıyla silindi.");
        }
    }
}
