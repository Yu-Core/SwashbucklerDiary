using Microsoft.AspNetCore.Mvc;
using SwashbucklerDiary.Server.Services;

namespace SwashbucklerDiary.Server.Controllers
{
    [Route("api/apiAuth")]
    [ApiController]
    public class ApiAuthController : ControllerBase
    {
        private readonly IApiAuthService _apiAuthService;

        public ApiAuthController(IApiAuthService apiAuthService)
        {
            _apiAuthService = apiAuthService;
        }

        [HttpPost("cookie")]
        public IActionResult SetCookie([FromBody] ApiKeyRequest request)
        {
            if (string.IsNullOrEmpty(request?.ApiKey))
            {
                return BadRequest(new { message = "apiKey cannot be empty" });
            }

            if (!_apiAuthService.ValidateApiKey(request.ApiKey))
            {
                return Unauthorized(new { message = "Invalid API key" });
            }

            var cookieValue = _apiAuthService.GenerateApiKey();

            Response.Cookies.Append("ApiAuth", cookieValue, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
            });

            return Ok("Cookie Generated");
        }
    }

    public class ApiKeyRequest
    {
        public string? ApiKey { get; set; }
    }
}
