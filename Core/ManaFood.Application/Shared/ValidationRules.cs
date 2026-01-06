using FluentValidation;

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
}
