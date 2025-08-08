using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace T2Informatik.SampleService.Tests.Context;

public class TestCase : IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestCase(string connectionString)
    {
        var connectionStringBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = Guid.NewGuid().ToString("N"),
        };

        _factory = new SampleServiceWebApplicationFactory(connectionStringBuilder.ToString());
        HttpClient = _factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SampleServiceDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public HttpClient HttpClient { get; }

    public ValueTask DisposeAsync() => _factory.DisposeAsync();
}
