using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Authentication;
using PrimeMarket.Services.Authentication;

namespace PrimeMarket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        [HttpPost]
        public async Task<IActionResult> LogInAsync(LogInRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.GetTokenAsync(request.Email, request.Password
                                                                , cancellationToken);

            return authResult is null ? BadRequest("invalid username/password") : Ok(authResult);
        }
    }
}
