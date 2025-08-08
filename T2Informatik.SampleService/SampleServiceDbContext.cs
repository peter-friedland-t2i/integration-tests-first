using Microsoft.EntityFrameworkCore;
using T2Informatik.SampleService.PersistenceModel;

namespace T2Informatik.SampleService;

public class SampleServiceDbContext(DbContextOptions<SampleServiceDbContext> options) : DbContext(options)
{
    public virtual required DbSet<TodoList> TodoList { get; set; }
    
    public virtual required DbSet<User> User { get; set; }
    
    public virtual required DbSet<SharedTodoList> SharedTodoList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoList>()
            .HasMany(t => t.SharedUsers)
            .WithMany(u => u.ReceivedSharedTodoLists)
            .UsingEntity<SharedTodoList>();

        modelBuilder.Entity<User>().HasMany(u => u.OwnedTodoLists).WithOne(t => t.Owner);
        
        base.OnModelCreating(modelBuilder);
    }
}