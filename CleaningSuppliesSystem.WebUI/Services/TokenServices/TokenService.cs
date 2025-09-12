// TokenService Implementation
using System.Security.Claims;

namespace CleaningSuppliesSystem.WebUI.Services.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private HttpContext? Context => _contextAccessor.HttpContext;

        public string GetUserToken => Context?.Request?.Cookies["AccessToken"] ?? string.Empty;

        public int GetUserId
        {
            get
            {
                var userIdClaim = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out var id) ? id : 0;
            }
        }

        public string GetUserRole => Context?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Customer";

        public string GetUserNameSurname => Context?.User?.FindFirst("fullName")?.Value ?? string.Empty;

        public string GetUserTheme => Context?.User?.FindFirst("theme")?.Value ?? "light";

        public void UpdateToken(string newToken)
        {
            if (Context?.Response != null && !Context.Response.HasStarted)
            {
                Context.Response.Cookies.Append("AccessToken", newToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(20) // Token expire süresine göre ayarlayın
                });
            }
        }

        public void ClearToken()
        {
            if (Context?.Response != null && !Context.Response.HasStarted)
            {
                Context.Response.Cookies.Delete("AccessToken");
                Context.Response.Cookies.Delete("RefreshToken");
            }
        }
    }
}