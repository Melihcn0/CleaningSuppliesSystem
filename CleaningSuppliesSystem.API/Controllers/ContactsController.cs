using AutoMapper;
using CleaningSuppliesSystem.API.Models;
using CleaningSuppliesSystem.Business.Abstract;
using CleaningSuppliesSystem.DTO.DTOs.Home.ContactDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleaningSuppliesSystem.API.Controllers
{
    [ApiExplorerSettings(GroupName = "Contacts")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;

        public ContactsController(IContactService contactService, IMapper mapper)
        {
            _contactService = contactService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var contacts = await _contactService.TGetListAsync();
            var result = _mapper.Map<List<ResultContactDto>>(contacts);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveContacts(int page = 1, int pageSize = 10)
        {
            // Toplam sayıları getir
            var totalActive = await _contactService.TGetActiveContactsCountAsync();
            var totalDeleted = await _contactService.TGetDeletedContactsCountAsync();

            // Aktif mesajları sayfalı al
            var activeContacts = await _contactService.TGetActiveContactsAsync();
            var pagedData = activeContacts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                ActiveCount = totalActive,
                DeletedCount = totalDeleted,
                PagedContacts = new PagedResponse<ResultContactDto>
                {
                    Data = pagedData,
                    TotalCount = totalActive,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalActive / (double)pageSize)
                }
            };

            return Ok(response);
        }

        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedContacts(int page = 1, int pageSize = 10)
        {
            // Toplam sayıları getir
            var totalActive = await _contactService.TGetActiveContactsCountAsync();
            var totalDeleted = await _contactService.TGetDeletedContactsCountAsync();

            // Silinmiş mesajları sayfalı al
            var deletedContacts = await _contactService.TGetDeletedContactsAsync();
            var pagedData = deletedContacts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                ActiveCount = totalActive,
                DeletedCount = totalDeleted,
                PagedContacts = new PagedResponse<ResultContactDto>
                {
                    Data = pagedData,
                    TotalCount = totalDeleted,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalDeleted / (double)pageSize)
                }
            };

            return Ok(response);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.TGetByIdAsync(id);
            if (contact == null)
                return NotFound("İletişim bulunamadı.");

            var result = _mapper.Map<ResultContactDto>(contact);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateContactDto createContactDto)
        {
            var (isSuccess, message, createdId) = await _contactService.TCreateContactAsync(createContactDto);

            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message = "Mesaj başarıyla oluşturuldu.", id = createdId });
        }

        [HttpPost("softdelete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _contactService.TSoftDeleteContactAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> SoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek mesaj bulunamadı.");

            var results = await _contactService.TSoftDeleteRangeContactAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili mesaj başarıyla çöp kutusuna taşındı.");
        }

        [HttpPost("UndoSoftDeleteMultiple")]
        public async Task<IActionResult> UndoSoftDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Geri alınacak mesaj bulunamadı.");

            var results = await _contactService.TUndoSoftDeleteRangeContactAsync(ids);

            var failed = results.Where(r => !r.IsSuccess).ToList();
            if (failed.Any())
            {
                var messages = string.Join("\n", failed.Select(f => f.Message));
                return BadRequest(messages);
            }

            return Ok("Seçili mesaj başarıyla geri alındı.");
        }
        [HttpPost("PermanentDeleteMultiple")]
        public async Task<IActionResult> PermanentDeleteMultipleAsync([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Silinecek mesaj bulunamadı.");

            await _contactService.TPermanentDeleteRangeContactAsync(ids);
            return Ok("Seçili mesaj çöp kutusundan kalıcı olarak silindi.");
        }
    }
}
