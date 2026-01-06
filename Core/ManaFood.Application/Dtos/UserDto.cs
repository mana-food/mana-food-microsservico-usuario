namespace ManaFood.Application.Dtos;

public record UserDto : BaseDto
{
    public required string Email { get; init; }
    public required string Name { get; init; }
    public required string Cpf { get; init; }
    public required string Password { get; init; }
    public required DateOnly Birthday { get; init; }
    public required int UserType { get; init; }
}