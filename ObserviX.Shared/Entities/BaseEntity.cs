namespace ObserviX.Shared.Entities;

public class BaseEntity
{
    protected BaseEntity()
    {
        DateTime nowUtc = DateTime.UtcNow;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; set; }
}