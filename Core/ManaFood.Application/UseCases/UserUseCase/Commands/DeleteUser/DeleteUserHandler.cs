using ManaFood.Application.Interfaces;
using MediatR;

namespace ManaFood.Application.UseCases.UserUseCase.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _repository;

    public DeleteUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetBy(c => c.Id == request.Id && !c.Deleted, cancellationToken);

        if (user == null)
            throw new ArgumentException($"Usuário com ID {request.Id} não encontrado");


        user.Deleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.Delete(user, cancellationToken);
        return Unit.Value;
    }
}
