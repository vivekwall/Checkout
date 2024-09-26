using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Dtos;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    [Route("processpayment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _paymentService.ProcessPayment(request);
        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        try
        {
            var payment = await _paymentService.Get(id);
            return new OkObjectResult(payment);
        }
        catch (NotFoundException ex) 
        {
            return NotFound(ex.Message);
        }
    }
}