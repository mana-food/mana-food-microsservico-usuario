using FluentAssertions;
using ManaFood.Application.UseCases.UserUseCase.Commands.UpdateUser;

namespace ManaFood.Tests.Validators;

public class UpdateUserValidatorTests
{
    private readonly UpdateUserValidator _validator;

    public UpdateUserValidatorTests()
    {
        _validator = new UpdateUserValidator();
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735", // CPF válido
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #region Name Validation Tests

    [Fact]
    public async Task Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Nome não pode ser vazio.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenNameIsNull()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            null,
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Nome não pode ser nulo.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenNameIsLessThan3Characters()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "ab",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Nome precisa ter no mínimo 3 caracteres.");
    }

    #endregion

    #region Email Validation Tests

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "",
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email não pode ser vazio.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailIsNull()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            null,
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email não pode ser nulo.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "invalid-email",
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email inválido.");
    }

    #endregion

    #region CPF Validation Tests

    [Fact]
    public async Task Validate_ShouldFail_WhenCpfIsEmpty()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cpf" && e.ErrorMessage == "CPF não pode ser vazio.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenCpfIsNull()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            null,
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cpf" && e.ErrorMessage == "CPF não pode ser nulo.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenCpfDoesNotHave11Characters()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "123456789",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cpf" && e.ErrorMessage == "CPF precisa ter 11 caracteres.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenCpfIsInvalid()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11111111111", // CPF inválido
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cpf" && e.ErrorMessage == "CPF inválido.");
    }

    #endregion

    #region Password Validation Tests

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Senha não pode ser vazia.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordIsNull()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            null,
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Senha não pode ser nula.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPasswordIsLessThan3Characters()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "ab",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Senha precisa ter no mínimo 3 caracteres.");
    }

    #endregion

    #region Birthday Validation Tests

    [Fact]
    public async Task Validate_ShouldFail_WhenBirthdayIsInFuture()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "password123",
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Birthday" && e.ErrorMessage == "Data de nascimento não pode ser no futuro.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserIsUnder18YearsOld()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "password123",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-17)), // 17 anos
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Birthday" && e.ErrorMessage == "Usuário deve ter no mínimo 18 anos de idade.");
    }

    #endregion

    #region UserType Validation Tests

    [Fact]
    public async Task Validate_ShouldFail_WhenUserTypeIsLessThan0()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            -1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserType" && e.ErrorMessage == "Tipo de usuário deve ser entre 0 e 4.");
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenUserTypeIsGreaterThan4()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            5
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserType" && e.ErrorMessage == "Tipo de usuário deve ser entre 0 e 4.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task Validate_ShouldPass_WhenUserTypeIsValid(int userType)
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            userType
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Id Validation Tests

    [Fact]
    public async Task Validate_ShouldPass_WhenIdIsValidGuid()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Guid.NewGuid(),
            "test@test.com",
            "Test User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            1
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion
}