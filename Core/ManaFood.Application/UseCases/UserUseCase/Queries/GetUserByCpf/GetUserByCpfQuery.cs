using MediatR;
using ManaFood.Application.Dtos;

namespace ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByCpf;

public sealed record GetUserByCpfQuery(string Cpf) : IRequest<UserDto>;
