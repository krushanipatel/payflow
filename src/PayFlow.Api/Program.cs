using PayFlow.Application.Payments;
using PayFlow.Infrastructure.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Dependency injection: "when someone asks for X, give them Y".
// Singleton = one shared in-memory store for the whole app (otherwise data would vanish after each request).
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
// Scoped = a new service object for each HTTP request.
builder.Services.AddScoped<CreatePaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();