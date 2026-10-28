using Microsoft.AspNetCore.Mvc;
using PayFlow.Api.Payments;
using PayFlow.Application.Payments;

namespace PayFlow.Api.Controllers;

[ApiController]
[Route("payments")]
public sealed class PaymentsController(CreatePaymentService createPayment, IPaymentRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
        CreatePaymentRequest request,
        CancellationToken ct)
    {
        try
        {
            var command = new CreatePaymentCommand(
                idempotencyKey, request.FromAccountId, request.ToAccountId, request.Amount, request.Currency);
            var result = await createPayment.HandleAsync(command, ct);
            var response = PaymentResponse.From(result.Payment);

            // New payment: 201 Created. Same key again: 200 OK with the SAME payment.
            return result.IsDuplicate
                ? Ok(response)
                : CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (ArgumentException ex) // a Domain rule was broken, e.g. amount <= 0
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var payment = await repository.GetByIdAsync(id, ct);
        return payment is null ? NotFound() : Ok(PaymentResponse.From(payment));
    }
}