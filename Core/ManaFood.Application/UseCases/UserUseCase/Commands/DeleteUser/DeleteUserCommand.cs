using MediatR;

namespace ManaFood.Application.UseCases.UserUseCase.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : IRequest<Unit>;