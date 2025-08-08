using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using T2Informatik.SampleService;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );
builder.Services.AddDbContext<SampleServiceDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
