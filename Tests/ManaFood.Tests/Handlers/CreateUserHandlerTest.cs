using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.Services;
using ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Moq;

namespace ManaFood.Tests.Handlers;

public class CreateUserHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserValidationService _validationService;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _validationService = new UserValidationService(_repositoryMock.Object);
        
        _handler = new CreateUserHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommand
        (
            "test@test.com",
            "Test User",
            "11144477735",
            "password",
            new DateOnly(2000, 1, 1),
            1
        );

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Name = command.Name,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = UserType.CUSTOMER
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Cpf = user.Cpf,
            Password = user.Password,
            Birthday = user.Birthday,
            UserType = (int)user.UserType
        };

        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        await _validationService.ValidateUniqueEmailAndCpfAsync(user, It.IsAny<CancellationToken>());
        _repositoryMock.Setup(r => r.Create(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(userDto);
        _repositoryMock.Verify(r => r.Create(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand
        (
            "existing@test.com",
            "Test User",
            "11144477735",
            "password",
            new DateOnly(2000, 1, 1),
            1
        );

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Name = command.Name,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = UserType.CUSTOMER
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Cpf = "98765432100",
            Name = "Existing User",
            Password = "pass",
            Birthday = new DateOnly(1990, 1, 1),
            UserType = UserType.CUSTOMER
        };

        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _repositoryMock.Setup(r => r.GetBy(It.Is<System.Linq.Expressions.Expression<Func<User, bool>>>(expr => expr.ToString().Contains("Email")), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(existingUser);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Esse email {command.Email} já está vinculado a um usuário. Escolha outro email.");
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCpfAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand
        (
            "new@test.com",
            "Test User",
            "11144477735",
            "password",
            new DateOnly(2000, 1, 1),
            1
        );

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Name = command.Name,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = UserType.CUSTOMER
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "different@test.com",
            Cpf = command.Cpf,
            Name = "Existing User",
            Password = "pass",
            Birthday = new DateOnly(1990, 1, 1),
            UserType = UserType.CUSTOMER
        };

        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _repositoryMock.SetupSequence(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User) // Email não existe
            .ReturnsAsync(existingUser); // CPF já existe

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Esse CPF {command.Cpf} já está vinculado a um usuário. Verifique se já não possui um usuário com este CPF.");
    }

    [Fact]
    public async Task Handle_ShouldMapCommandToUser_Correctly()
    {
        // Arrange
        var command = new CreateUserCommand
        (
            "test@test.com",
            "Test User",
            "11144477735",
            "password",
            new DateOnly(2000, 1, 1),
            1
        );

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Name = command.Name,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = UserType.CUSTOMER
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Cpf = user.Cpf,
            Password = user.Password,
            Birthday = user.Birthday,
            UserType = (int)user.UserType
        };

        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User);
        _repositoryMock.Setup(r => r.Create(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map<User>(command), Times.Once);
        _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
    }
}