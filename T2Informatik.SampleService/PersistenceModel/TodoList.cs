using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace T2Informatik.SampleService.PersistenceModel;

[Table("TodoList")]
public class TodoList
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(8192)]
    public required string Name { get; set; }

    public virtual User Owner { get; set; } = null!;
    
    [ForeignKey(nameof(Owner))]
    public int OwnerId { get; set; }
    
    public virtual ICollection<User> SharedUsers { get; set; } =  new List<User>();
}