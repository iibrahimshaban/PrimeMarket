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
        [HttpPost("")]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest request, CancellationToken cancellationToken)
        {
            var Result = await _authService.GetTokenAsync(request.Email, request.Password
                                                                , cancellationToken);

            return Result.IsSuccess ? Ok(Result.Value) : Result.ToProblem();
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterReq request, CancellationToken cancellationToken)
        {
            var Result = await _authService.RegisterAsync(request, cancellationToken);

            return Result.IsSuccess ? Ok(Result.Value) : Result.ToProblem();
        }
    }
}
