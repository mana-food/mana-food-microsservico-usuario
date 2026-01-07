using FluentValidation;
using ManaFood.Application.Shared;

namespace ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name).ValidUserName();
        RuleFor(x => x.Email).ValidEmail();
        RuleFor(x => x.Cpf).ValidCpf();
        RuleFor(x => x.Password).ValidPassword();
        RuleFor(x => x.Birthday).ValidBirthday();
        RuleFor(x => x.UserType).ValidUserType();
    }
}