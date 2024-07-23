namespace Hermes.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}