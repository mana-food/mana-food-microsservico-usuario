namespace ManaFood.Application.Dtos;

public abstract record BaseDto
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid UpdatedBy { get; init; }
    public bool Deleted { get; set; }
}