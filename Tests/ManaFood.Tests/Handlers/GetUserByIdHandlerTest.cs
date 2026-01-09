using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserById;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Moq;
using Xunit;

namespace ManaFood.Tests.Handlers;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserByIdHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        var user = new User
        {
            Id = userId,
            Email = "test@test.com",
            Name = "Test",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER
        };

        var userDto = new UserDto
        {
            Id = userId,
            Email = "test@test.com",
            Name = "Test",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = (int)UserType.CUSTOMER
        };

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(userDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetBy(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(null as User);
        _mapperMock.Setup(m => m.Map<UserDto>(null)).Returns(null as UserDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}