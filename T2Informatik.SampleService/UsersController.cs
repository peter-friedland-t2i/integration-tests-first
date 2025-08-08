using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T2Informatik.SampleService.PersistenceModel;

namespace T2Informatik.SampleService.Controllers;

public record UserWriteViewModel(string UserName);
public record TodoListReadViewModel(int Id, string Name, string? SharedFromUserName);
public record TodoListWriteViewModel(string Name);
public record SharedTodoListWriteViewModel(int TodoListId, int SharingUserId);
public record UserReadViewModel(int Id, string UserName);

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(SampleServiceDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var viewModels = (await dbContext.User.ToListAsync())
            .Select(user => new UserReadViewModel(user.Id, user.UserName));
        
        return Ok(viewModels);        
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody]UserWriteViewModel viewModel)
    {
        await dbContext.User.AddAsync(new User
        {
            UserName = viewModel.UserName
        });
        await dbContext.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPost("{user-id:int}/TodoLists")]
    public async Task<IActionResult> AddTodoList([FromRoute(Name = "user-id")]int userId, [FromBody] TodoListWriteViewModel viewModel)
    {
        var user = await dbContext.User.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return BadRequest();
        
        user.OwnedTodoLists.Add(new TodoList
        {
            Name = viewModel.Name
        });
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPost("{user-id:int}/SharedTodoLists")]
    public async Task<IActionResult> AddSharedTodoList([FromRoute(Name = "user-id")]int userId, [FromBody] SharedTodoListWriteViewModel viewModel)
    {
        var todoListToShare = await dbContext.TodoList.FirstOrDefaultAsync(t => t.Id == viewModel.TodoListId && t.OwnerId == userId);
        if (todoListToShare == null) return NotFound();
        
        var targetUser = await dbContext
            .User
            .Include(u => u.ReceivedSharedTodoLists)
            .FirstOrDefaultAsync(u => u.Id == viewModel.SharingUserId);
        if (targetUser == null) return BadRequest();

        targetUser.ReceivedSharedTodoLists.Add(todoListToShare);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{user-id:int}/TodoLists")]
    public async Task<IActionResult> GetAllTodoLists([FromRoute(Name = "user-id")] int userId)
    {
        var usersWithLists = await dbContext.User.Include(u => u.OwnedTodoLists)
            .Include(u => u.ReceivedSharedTodoLists)
            .ThenInclude(u => u.Owner)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (usersWithLists == null) return BadRequest();

        var allTodoLists = usersWithLists
            .OwnedTodoLists
                .Select(t => new TodoListReadViewModel(t.Id, t.Name, null))
            .Concat(usersWithLists
                .ReceivedSharedTodoLists
                .Select(s => new TodoListReadViewModel(s.Id, s.Name, s.Owner.UserName)
        ));
        
        return Ok(allTodoLists);
    }
}