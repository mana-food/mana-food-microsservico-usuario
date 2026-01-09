using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Commands.UpdateUser;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Moq;

namespace ManaFood.Tests.Handlers;

public class UpdateUserHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserValidationService _validationService; 
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _validationService = new UserValidationService(_repositoryMock.Object);
        
        _handler = new UpdateUserHandler(
            _repositoryMock.Object,
            _mapperMock.Object,
            _validationService
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand
        (
            userId,
            "updated@test.com",
            "Updated User",
            "11144477735",
            "12345678",
            new DateOnly(2000, 1, 1),
            1
        );

        var existingUser = new User
        {
            Id = userId,
            Email = "old@test.com",
            Name = "Old User",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER
        };

        var userDto = new UserDto
        {
            Id = userId,
            Email = command.Email,
            Name = command.Name,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = command.UserType
        };

        var callCount = 0;
        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), 
                It.IsAny<CancellationToken>(), 
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<Func<User, bool>> predicate, 
                          CancellationToken ct, 
                          System.Linq.Expressions.Expression<Func<User, object>>[] includes) =>
            {
                callCount++;
                if (callCount == 1) return existingUser;
                return null;
            });

        _repositoryMock.Setup(r => r.Update(existingUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mapperMock.Setup(m => m.Map<UserDto>(existingUser)).Returns(userDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        existingUser.Email.Should().Be(command.Email);
        existingUser.Name.Should().Be(command.Name);
        _repositoryMock.Verify(r => r.Update(existingUser, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand
        (
            Guid.NewGuid(),
            "test@test.com",
            "Test",
            "11144477735",
            "12345678",
            new DateOnly(2000, 1, 1),
            1
        );

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), 
                It.IsAny<CancellationToken>(), 
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Usuário com ID {command.Id} não encontrado");
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEmailAlreadyExistsForDifferentUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUserId = Guid.NewGuid();
        var command = new UpdateUserCommand
        (
            userId,
            "existing@test.com",
            "Updated User",
            "11144477735",
            "12345678",
            new DateOnly(2000, 1, 1),
            1
        );

        var currentUser = new User
        {
            Id = userId,
            Email = "old@test.com",
            Name = "Current User",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER
        };

        var userWithSameEmail = new User
        {
            Id = existingUserId,
            Email = "existing@test.com",
            Name = "Other User",
            Cpf = "98765432100",
            Password = "password",
            Birthday = new DateOnly(1990, 1, 1),
            UserType = UserType.CUSTOMER
        };

        var callCount = 0;
        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), 
                It.IsAny<CancellationToken>(), 
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<Func<User, bool>> predicate, 
                          CancellationToken ct, 
                          System.Linq.Expressions.Expression<Func<User, object>>[] includes) =>
            {
                callCount++;
                if (callCount == 1) return currentUser; // Usuário atual existe
                if (callCount == 2) return userWithSameEmail; // Email já está em uso
                return null;
            });

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Esse email {command.Email} já está vinculado a um usuário. Escolha outro email.");
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllProperties_WhenCommandIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newBirthday = new DateOnly(1995, 6, 15);
        var command = new UpdateUserCommand
        (
            userId,
            "newemail@test.com",
            "New Name",
            "22255588899",
            "newpassword123",
            newBirthday,
            2
        );

        var existingUser = new User
        {
            Id = userId,
            Email = "old@test.com",
            Name = "Old Name",
            Cpf = "11144477735",
            Password = "oldpassword",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER
        };

        var userDto = new UserDto
        {
            Id = userId,
            Email = command.Email,
            Name = command.Name,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = command.UserType
        };

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), 
                It.IsAny<CancellationToken>(), 
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(existingUser);

        _repositoryMock.Setup(r => r.Update(existingUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        _mapperMock.Setup(m => m.Map<UserDto>(existingUser)).Returns(userDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        existingUser.Email.Should().Be("newemail@test.com");
        existingUser.Name.Should().Be("New Name");
        existingUser.Cpf.Should().Be("22255588899");
        existingUser.Password.Should().Be("newpassword123");
        existingUser.Birthday.Should().Be(newBirthday);
        existingUser.UserType.Should().Be((UserType)2);
    }
}
