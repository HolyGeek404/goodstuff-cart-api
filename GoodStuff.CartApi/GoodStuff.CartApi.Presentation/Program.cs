using Azure.Identity;
using GoodStuff.CartApi.Application.Features.Commands.AddCart;
using GoodStuff.CartApi.Application.Services;
using GoodStuff.CartApi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCartCommand).Assembly));
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Configuration.AddAzureKeyVault(new Uri(builder.Configuration.GetSection("AzureAd")["KvUrl"]!), new DefaultAzureCredential());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
