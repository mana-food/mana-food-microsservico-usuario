using FluentValidation;
using ManaFood.Application.Utils;

namespace ManaFood.Application.Shared;

public static class ValidationRules
{
    public static IRuleBuilderOptions<T, string> RequiredString<T>(this IRuleBuilder<T, string> ruleBuilder, string field)
    {
        return ruleBuilder
            .NotNull().WithMessage($"{field} não pode ser nulo.")
            .NotEmpty().WithMessage($"{field} não pode ser vazio.")
            .MinimumLength(3).WithMessage($"{field} precisa ter no mínimo 3 caracteres.");
    }

    public static IRuleBuilderOptions<T, Guid> RequiredGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder, string field)
    {
        return ruleBuilder
            .Must(guid => guid != Guid.Empty).WithMessage($"{field} não pode ser um Guid vazio.");
    }
    
    public static IRuleBuilderOptions<T, int> RequiredPagination<T>(this IRuleBuilder<T, int> ruleBuilder, string field)
    {
        return ruleBuilder
            .GreaterThan(0).WithMessage($"{field} deve ser maior que zero.");
    }

    public static IRuleBuilderOptions<T, string> ValidUserName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Nome não pode ser vazio.")
            .NotNull().WithMessage("Nome não pode ser nulo.")
            .MinimumLength(3).WithMessage("Nome precisa ter no mínimo 3 caracteres.");
    }

    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email não pode ser vazio.")
            .NotNull().WithMessage("Email não pode ser nulo.")
            .EmailAddress().WithMessage("Email inválido.");
    }

    public static IRuleBuilderOptions<T, string> ValidCpf<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("CPF não pode ser vazio.")
            .NotNull().WithMessage("CPF não pode ser nulo.")
            .Length(11).WithMessage("CPF precisa ter 11 caracteres.")
            .Must(CpfUtils.IsValidCpf).WithMessage("CPF inválido.");
    }

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Senha não pode ser vazia.")
            .NotNull().WithMessage("Senha não pode ser nula.")
            .MinimumLength(3).WithMessage("Senha precisa ter no mínimo 3 caracteres.");
    }

    public static IRuleBuilderOptions<T, DateOnly> ValidBirthday<T>(this IRuleBuilder<T, DateOnly> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Data de nascimento não pode ser vazia.")
            .NotNull().WithMessage("Data de nascimento não pode ser nula.")
            .Must(birthday => birthday <= DateOnly.FromDateTime(DateTime.Today)).WithMessage("Data de nascimento não pode ser no futuro.")
            .Must(BirthdayUtils.IsValidBirthday).WithMessage("Usuário deve ter no mínimo 18 anos de idade.");
    }

    public static IRuleBuilderOptions<T, int> ValidUserType<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .NotNull().WithMessage("Tipo de usuário não pode ser nulo.")
            .InclusiveBetween(0, 4).WithMessage("Tipo de usuário deve ser entre 0 e 4.");
    }
}