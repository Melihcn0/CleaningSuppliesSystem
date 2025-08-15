using System.Security.Claims;

namespace CleaningSuppliesSystem.WebUI.Services.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpContext _context;

        public TokenService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _context = _contextAccessor.HttpContext;
        }

        public string GetUserToken => _context?.Request?.Cookies["AccessToken"];

        public int GetUserId => int.TryParse(_context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        public string GetUserRole => _context?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

        public string GetUserNameSurname => _context?.User?.FindFirst("fullName")?.Value ?? "";
        public string GetUserTheme => _context?.User?.FindFirst("theme")?.Value ?? "light";
    }
}