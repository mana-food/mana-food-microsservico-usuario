using ManaFood.Application.Interfaces;
using ManaFood.Domain.Entities;

public class UserValidationService
{
    private readonly IUserRepository _repository;

    public UserValidationService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task ValidateUniqueEmailAndCpfAsync(User user, CancellationToken cancellationToken)
    {
        // Verifica se o email já pertence a outro usuário
        var existingUser = await _repository.GetBy(u => u.Email == user.Email, cancellationToken);
        if (existingUser != null && existingUser.Id != user.Id)
        {
            throw new Exception($"Esse email {user.Email} já está vinculado a um usuário. Escolha outro email.");
        }

        // Verifica se o CPF já pertence a outro usuário
        var existingUserByCpf = await _repository.GetBy(u => u.Cpf == user.Cpf, cancellationToken);
        if (existingUserByCpf != null && existingUserByCpf.Id != user.Id)
        {
            throw new Exception($"Esse CPF {user.Cpf} já está vinculado a um usuário. Verifique se já não possui um usuário com este CPF.");
        }
    }
}