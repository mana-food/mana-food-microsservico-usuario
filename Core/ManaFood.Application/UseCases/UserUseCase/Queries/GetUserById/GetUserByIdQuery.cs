using MediatR;
using ManaFood.Application.Dtos;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
