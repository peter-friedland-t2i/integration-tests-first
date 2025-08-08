using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Informatik.SampleService.PersistenceModel;

[Table("User")]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(256)]
    public required string UserName { get; set; }

    public virtual ICollection<TodoList> OwnedTodoLists { get; set; } = new List<TodoList>();

    public virtual ICollection<TodoList> ReceivedSharedTodoLists { get; set; } = new List<TodoList>();
}