using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = Environment.GetEnvironmentVariable("REDIS_CONNSTR")!;
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();