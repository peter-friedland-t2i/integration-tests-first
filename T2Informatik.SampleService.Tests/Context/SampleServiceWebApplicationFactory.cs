using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace T2Informatik.SampleService.Tests.Context;

internal sealed class SampleServiceWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configurationValues = new[]
        {
            new KeyValuePair<string, string?>("ConnectionStrings:Database", connectionString),
        };

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddInMemoryCollection(configurationValues)
            .Build();

        builder
            .UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
                configurationBuilder.AddInMemoryCollection(configurationValues)
            );
    }
}
