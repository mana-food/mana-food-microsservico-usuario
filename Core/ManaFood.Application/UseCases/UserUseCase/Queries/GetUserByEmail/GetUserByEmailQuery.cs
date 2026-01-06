using MediatR;
using ManaFood.Application.Dtos;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByEmail;

public sealed record GetUserByEmailQuery(string Email) : IRequest<UserDto>;
