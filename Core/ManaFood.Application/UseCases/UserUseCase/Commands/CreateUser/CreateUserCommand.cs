using MediatR;
using ManaFood.Application.Dtos;

namespace ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string Name,
    string Cpf,
    string Password,
    DateOnly Birthday,
    int UserType) : IRequest<UserDto>;