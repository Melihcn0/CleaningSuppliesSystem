using CleaningSuppliesSystem.WebUI.Services.TokenServices;
using System.Net.Http.Headers;

namespace CleaningSuppliesSystem.WebUI.Handlers
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;

        public TokenHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _tokenService.GetUserToken;
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
