using FluentAssertions;
using Reqnroll;
using T2Informatik.SampleService.Controllers;
using T2Informatik.SampleService.Tests.Context;

namespace T2Informatik.SampleService.Tests;

[Binding]
public class Steps(HttpClient httpClient)
{
    [Given("The system has the following users with the following owned todo lists")]
    public async Task GivenTheSystemHasTheFollowingUsersWithTheFollowingOwnedTodoLists(Table table)
    {
        foreach (var row in table.Rows)
        {
            await httpClient.PostAsync("api/Users", new UserWriteViewModel(row["UserName"]));
            
            var userId = await GetUserIdFromNameAsync(row["UserName"]);

            var todoLists = row["Owned Todo Lists"]
                .Split(',')
                .Select(t => t.Trim(' ', '"'))
                .Where(t => !string.IsNullOrWhiteSpace(t));

            foreach (var todoListName in todoLists)
            {
                await httpClient.PostAsync($"api/Users/{userId}/TodoLists",
                    new TodoListWriteViewModel(todoListName));
            }
        }
    }

    [When("{string} has created the todo list {string}")]
    public async Task WhenHasCreatedTheTodoList(string userName, string todoListName)
    {
        var userId = await GetUserIdFromNameAsync(userName);
        
        await httpClient.PostAsync($"api/Users/{userId}/TodoLists",
            new TodoListWriteViewModel(todoListName));
    }

    [Then("{string} has the todo lists {string} and {string}")]
    public async Task ThenHasTheTodoListsAnd(string userName, string todoListName1, string todoListName2)
    {
        var userId = await GetUserIdFromNameAsync(userName);

        var todoListNames = (await httpClient.GetAsync<TodoListReadViewModel[]>($"api/Users/{userId}/TodoLists"))
            .Select(t => t.Name)
            .OrderBy(t => t)
            .ToArray();

        todoListNames
            .Should()
            .BeEquivalentTo(
                new[] { todoListName1, todoListName2 }
                    .OrderBy(t => t));
    }

    [Then("{string} has the todo list {string}")]
    public async Task ThenHasTheTodoList(string userName, string todoListName1)
    {
        var userId = await GetUserIdFromNameAsync(userName);

        var todoLists = await httpClient.GetAsync<TodoListReadViewModel[]>($"api/Users/{userId}/TodoLists");
        
        todoLists.Length.Should().Be(1);
        todoLists[0].Name.Should().Be(todoListName1);
    }

    [When("{string} shares {string} with {string}")]
    public async Task WhenSharesWith(string userName, string todoListName, string sharingUserName)
    {
        var sourceUserId = await GetUserIdFromNameAsync(userName);
        var todoListId = (await httpClient.GetAsync<TodoListReadViewModel[]>($"api/Users/{sourceUserId}/TodoLists"))
            .Single(t => t.Name == todoListName).Id;
        var targetUserId = await GetUserIdFromNameAsync(sharingUserName);
        
        await httpClient.PostAsync($"api/Users/{sourceUserId}/SharedTodoLists", new SharedTodoListWriteViewModel(todoListId, targetUserId));
    }

    [Then("{string} has the overall todo lists")]
    public async Task ThenHasTheOverallTodoLists(string userName, Table table)
    {
        var userId = await GetUserIdFromNameAsync(userName);
        var todoLists = (await httpClient.GetAsync<TodoListReadViewModel[]>($"api/Users/{userId}/TodoLists"))
            .OrderBy(t => t.Name)
            .ToArray();
        
        var expectedList = table.Rows
            .Select(r => new TodoListReadViewModel(
                0, 
                r["Name"], 
                string.IsNullOrEmpty(r["Shared From"]) ? null : r["Shared From"]))
            .OrderBy(r => r.Name)
            .ToArray();
        
        todoLists.Length.Should().Be(expectedList.Length);

        for (var i = 0; i < expectedList.Length; i++)
        {
            todoLists[i].Name.Should().Be(expectedList[i].Name);
            todoLists[i].SharedFromUserName.Should().Be(expectedList[i].SharedFromUserName);
        }
    }

    private async Task<int> GetUserIdFromNameAsync(string userName) =>
        (await httpClient.GetAsync<UserReadViewModel[]>("api/Users"))
        .Single(u => u.UserName == userName).Id;
}