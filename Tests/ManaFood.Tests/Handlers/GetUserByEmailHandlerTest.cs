using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByEmail;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Moq;

namespace ManaFood.Tests.Handlers;

public class GetUserByEmailHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByEmailHandler _handler;

    public GetUserByEmailHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _handler = new GetUserByEmailHandler(
            _repositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExistsWithEmail()
    {
        // Arrange
        var email = "test@test.com";
        var query = new GetUserByEmailQuery(email);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER,
            Deleted = false
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

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(userDto);
        result.Email.Should().Be(email);
        result.Name.Should().Be(user.Name);
        
        _repositoryMock.Verify(r => r.GetBy(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()), 
            Times.Once);
        
        _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@test.com";
        var query = new GetUserByEmailQuery(email);

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((User?)null!);

        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns((UserDto?)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        
        _repositoryMock.Verify(r => r.GetBy(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserIsDeleted()
    {
        // Arrange
        var email = "deleted@test.com";
        var query = new GetUserByEmailQuery(email);

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((User?)null!);

        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns((UserDto?)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        
        _repositoryMock.Verify(r => r.GetBy(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("user1@test.com")]
    [InlineData("user2@example.com")]
    [InlineData("admin@company.com")]
    public async Task Handle_ShouldReturnCorrectUser_ForDifferentEmails(string email)
    {
        // Arrange
        var query = new GetUserByEmailQuery(email);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER,
            Deleted = false
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

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectPredicate()
    {
        // Arrange
        var email = "test@test.com";
        var query = new GetUserByEmailQuery(email);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER,
            Deleted = false
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

        System.Linq.Expressions.Expression<Func<User, bool>>? capturedPredicate = null;

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .Callback<System.Linq.Expressions.Expression<Func<User, bool>>, CancellationToken, System.Linq.Expressions.Expression<Func<User, object>>[]>(
                (predicate, ct, includes) => capturedPredicate = predicate)
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        capturedPredicate.Should().NotBeNull();
        
        var compiledPredicate = capturedPredicate.Compile();
        
        var matchingUser = new User 
        { 
            Email = email, 
            Name = "Test",
            Cpf = "11144477735",
            Password = "password",
            Deleted = false 
        };
        compiledPredicate(matchingUser).Should().BeTrue();
        
        var differentEmailUser = new User 
        { 
            Email = "other@test.com", 
            Name = "Test",
            Cpf = "11144477735",
            Password = "password",
            Deleted = false 
        };
        compiledPredicate(differentEmailUser).Should().BeFalse();
        
        var deletedUser = new User 
        { 
            Email = email, 
            Name = "Test",
            Cpf = "11144477735",
            Password = "password",
            Deleted = true 
        };
        compiledPredicate(deletedUser).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldHandleCancellationToken()
    {
        // Arrange
        var email = "test@test.com";
        var query = new GetUserByEmailQuery(email);
        var cancellationToken = new CancellationToken();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.CUSTOMER,
            Deleted = false
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

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        
        _repositoryMock.Verify(r => r.GetBy(
            It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
            cancellationToken,
            It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()), 
            Times.Once);
    }
}