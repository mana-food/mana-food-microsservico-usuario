using FluentAssertions;
using ManaFood.Application.Interfaces;
using ManaFood.Application.Services;
using ManaFood.Domain.Entities;
using Moq;

namespace ManaFood.Tests.Services;

public class UserValidationServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserValidationService _service;

    public UserValidationServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _service = new UserValidationService(_repositoryMock.Object);
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldNotThrow_WhenEmailAndCpfAreUnique()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        var existingUser = new User
        {
            Id = existingUserId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Existing",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.Setup(r => r.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(expr => expr.ToString().Contains("Email")), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(existingUser);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Esse email {user.Email} já está vinculado a um usuário. Escolha outro email.");
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldThrow_WhenCpfAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        var existingUser = new User
        {
            Id = existingUserId,
            Email = "other@test.com",
            Cpf = "11144477735",
            Name = "Existing",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.SetupSequence(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User) 
            .ReturnsAsync(existingUser); 

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Esse CPF {user.Cpf} já está vinculado a um usuário. Verifique se já não possui um usuário com este CPF.");
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldNotThrow_WhenEmailBelongsToSameUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldNotThrow_WhenCpfBelongsToSameUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.SetupSequence(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User)
            .ReturnsAsync(user);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldThrow_WhenBothEmailAndCpfExistForDifferentUsers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUserId1 = Guid.NewGuid();
        
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        var existingUserWithEmail = new User
        {
            Id = existingUserId1,
            Email = "test@test.com",
            Cpf = "98765432100",
            Name = "Existing",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.Setup(r => r.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(expr => expr.ToString().Contains("Email")), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(existingUserWithEmail);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert - Deve lançar exceção para email (primeira validação)
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Esse email {user.Email} já está vinculado a um usuário. Escolha outro email.");
    }

    [Fact]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldNotThrow_WhenUpdatingUserWithSameEmailAndCpf()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Cpf = "11144477735",
            Name = "Test Updated",
            Password = "newpass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        // Simula que o email e CPF pertencem ao mesmo usuário sendo atualizado
        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(user);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("user1@test.com", "11144477735")]
    [InlineData("user2@test.com", "22255588899")]
    [InlineData("user3@test.com", "33366699900")]
    public async Task ValidateUniqueEmailAndCpfAsync_ShouldNotThrow_ForDifferentUniqueUsers(string email, string cpf)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Cpf = cpf,
            Name = "Test",
            Password = "pass",
            Birthday = new DateOnly(2000, 1, 1)
        };

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User);

        // Act
        var act = async () => await _service.ValidateUniqueEmailAndCpfAsync(user, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}