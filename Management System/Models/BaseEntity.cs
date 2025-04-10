namespace Management_System.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public bool IsDeleted { get; set; }

}
