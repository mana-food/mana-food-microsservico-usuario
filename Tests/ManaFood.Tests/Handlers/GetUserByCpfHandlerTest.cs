using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByCpf;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Moq;

namespace ManaFood.Tests.Handlers;

public class GetUserByCpfHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByCpfHandler _handler;

    public GetUserByCpfHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        
        _handler = new GetUserByCpfHandler(
            _repositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnUserDto_WhenUserExistsWithCpf()
    {
        // Arrange
        var cpf = "11144477735";
        var query = new GetUserByCpfQuery(cpf);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            Cpf = cpf,
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
        result.Cpf.Should().Be(cpf);
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
        var cpf = "98765432100";
        var query = new GetUserByCpfQuery(cpf);

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((User)null);

        _mapperMock.Setup(m => m.Map<UserDto>(null))
            .Returns((UserDto)null);

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
        var cpf = "11144477735";
        var query = new GetUserByCpfQuery(cpf);

        _repositoryMock.Setup(r => r.GetBy(
                It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<User, object>>[]>()))
            .ReturnsAsync((User)null);

        _mapperMock.Setup(m => m.Map<UserDto>(null))
            .Returns((UserDto)null);

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
    [InlineData("11144477735")]
    [InlineData("98765432100")]
    [InlineData("11122233344")]
    public async Task Handle_ShouldReturnCorrectUser_ForDifferentCpfs(string cpf)
    {
        // Arrange
        var query = new GetUserByCpfQuery(cpf);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            Cpf = cpf,
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
        result.Cpf.Should().Be(cpf);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectPredicate()
    {
        // Arrange
        var cpf = "11144477735";
        var query = new GetUserByCpfQuery(cpf);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            Cpf = cpf,
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

        System.Linq.Expressions.Expression<Func<User, bool>> capturedPredicate = null;

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
            Email = "test@test.com",
            Name = "Test",
            Cpf = cpf,
            Password = "password",
            Deleted = false 
        };
        compiledPredicate(matchingUser).Should().BeTrue();
        
        var differentCpfUser = new User 
        { 
            Email = "test@test.com",
            Name = "Test",
            Cpf = "98765432100",
            Password = "password",
            Deleted = false 
        };
        compiledPredicate(differentCpfUser).Should().BeFalse();
        
        var deletedUser = new User 
        { 
            Email = "test@test.com",
            Name = "Test",
            Cpf = cpf,
            Password = "password",
            Deleted = true 
        };
        compiledPredicate(deletedUser).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldHandleCancellationToken()
    {
        // Arrange
        var cpf = "11144477735";
        var query = new GetUserByCpfQuery(cpf);
        var cancellationToken = new CancellationToken();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            Cpf = cpf,
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