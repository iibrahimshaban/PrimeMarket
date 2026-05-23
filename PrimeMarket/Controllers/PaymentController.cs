using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace PrimeMarket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(ApplicationDbContext context, IConfiguration config) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly IConfiguration _config = config;
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            _config["Stripe:WebhookSecret"]
        );

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentRef == intent!.Id);
            if (order is not null)
            {
                order.Status = OrderStatus.Confirmed;
                await _context.SaveChangesAsync();
            }
        }

        return Ok();
    }
}
