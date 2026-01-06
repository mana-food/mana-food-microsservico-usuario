using FluentAssertions;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Commands.DeleteUser;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using MediatR;
using Moq;

namespace ManaFood.Tests.Handlers;

public class DeleteUserHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteUserHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        var existingUser = new User
        {
            Id = userId,
            Email = "test@test.com",
            Name = "Test",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER,
            Deleted = false
        };

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(existingUser);
        _repositoryMock.Setup(r => r.Delete(existingUser, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        existingUser.Deleted.Should().BeTrue();
        _repositoryMock.Verify(r => r.Delete(existingUser, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((User)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Usuário com ID {command.Id} não encontrado");
    }
}