using GoodStuff.CartApi.Application.Features.Commands.AddCart;
using GoodStuff.CartApi.Application.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCartCommand).Assembly));
builder.Services.AddScoped<CartService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = Environment.GetEnvironmentVariable("REDIS_CONNSTR")!;
    return ConnectionMultiplexer.Connect(configuration);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
