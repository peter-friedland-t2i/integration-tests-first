using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Informatik.SampleService.PersistenceModel;

[Table("SharedTodoLists")]
public class SharedTodoList
{
    [Key]
    public int Id { get; set; }

    public virtual TodoList TodoList { get; set; } = null!;
    
    [ForeignKey(nameof(TodoList))]
    public int TodoListId { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
}