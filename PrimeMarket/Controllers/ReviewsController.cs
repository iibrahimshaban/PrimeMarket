using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimeMarket.Contracts.Reviews;
using PrimeMarket.Helpers;
using PrimeMarket.Services;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    private readonly IReviewService _reviewService = reviewService;
    [HttpPost("{productId}")]
    [Authorize]
    public async Task<IActionResult> AddReview(int productId, [FromBody] AddReviewRequest request, CancellationToken ct)
    {
        var UserId = User.GetUserId();
        var result = await _reviewService.AddReviewAsync(productId, UserId, request, ct);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
