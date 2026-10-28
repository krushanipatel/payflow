using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PayFlow.Api.Payments;

namespace PayFlow.Api.Tests;

public class PaymentsEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static HttpRequestMessage PostPayment(string? idempotencyKey, decimal amount = 100m)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/payments")
        {
            Content = JsonContent.Create(new CreatePaymentRequest(Guid.NewGuid(), Guid.NewGuid(), amount, "CAD"))
        };
        if (idempotencyKey is not null)
            request.Headers.Add("Idempotency-Key", idempotencyKey);
        return request;
    }

    [Fact]
    public async Task Post_SameIdempotencyKeyTwice_CreatesOnlyOnePayment()
    {
        var key = $"test-{Guid.NewGuid()}";

        var first = await _client.SendAsync(PostPayment(key));
        var second = await _client.SendAsync(PostPayment(key));

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        var firstPayment = await first.Content.ReadFromJsonAsync<PaymentResponse>();
        var secondPayment = await second.Content.ReadFromJsonAsync<PaymentResponse>();
        Assert.Equal(firstPayment!.Id, secondPayment!.Id);
        Assert.Equal("Completed", firstPayment.Status);
    }

    [Fact]
    public async Task Post_NegativeAmount_Returns400()
    {
        var response = await _client.SendAsync(PostPayment($"test-{Guid.NewGuid()}", amount: -5m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithoutIdempotencyKey_Returns400()
    {
        var response = await _client.SendAsync(PostPayment(idempotencyKey: null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}