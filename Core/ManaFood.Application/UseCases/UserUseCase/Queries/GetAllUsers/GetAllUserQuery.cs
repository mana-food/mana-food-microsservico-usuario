using MediatR;
using ManaFood.Application.Dtos;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<List<UserDto>>;
