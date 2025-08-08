using Reqnroll;
using Reqnroll.BoDi;

namespace T2Informatik.SampleService.Tests.Context;

[Binding]
public sealed class Hooks(ScenarioContext scenarioContext, IObjectContainer container)
{
    private static TestBed? _testBed;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _testBed = new TestBed();
        await _testBed.InitializeAsync();
    }

    [BeforeScenario]
    public async Task BeforeScenarioAsync()
    {
        if (_testBed == null)
            throw new ArgumentNullException(nameof(_testBed));

        var testCase = await _testBed.CreateTestCaseAsync();
        scenarioContext.Set(testCase);
        container.RegisterInstanceAs(testCase.HttpClient);
        
        await testCase.InitializeAsync();
    }

    [AfterScenario]
    public async Task AfterScenarioAsync()
    {
        var testCase = scenarioContext.Get<TestCase>();
        if (testCase != null)
        {
            await testCase.DisposeAsync();
        }
    }

    [AfterTestRun]
    public static async Task AfterTestRunAsync()
    {
        if (_testBed != null)
        {
            await _testBed.DisposeAsync();
        }
        _testBed = null;
    }
}