using Testcontainers.PostgreSql;

namespace T2Informatik.SampleService.Tests.Context;

public class TestBed : IAsyncLifetime
{
    private PostgreSqlContainer? _postgreSqlContainer;

    public async Task<TestCase> CreateTestCaseAsync()
    {
        if (_postgreSqlContainer == null)
            throw new NullReferenceException("Postgresql Container not build");

        await _postgreSqlContainer.StartAsync();
        var testContext = new TestCase(_postgreSqlContainer.GetConnectionString());

        return testContext;
    }

    public Task InitializeAsync()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17.5")
            .WithName("SampleServiceDatabaseContainer")
            .WithUsername("test")
            .WithPassword("test")
            .Build();
        return Task.CompletedTask;
    }

    public Task DisposeAsync() =>
        _postgreSqlContainer != null
            ? _postgreSqlContainer.DisposeAsync().AsTask()
            : Task.CompletedTask;
}
