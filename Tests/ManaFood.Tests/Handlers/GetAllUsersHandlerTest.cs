using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetAllUsers;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Moq;

namespace ManaFood.Tests.Handlers;

public class GetAllUsersHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllUsersdHandler _handler;

    public GetAllUsersHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllUsersdHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfUsers()
    {
        // Arrange
        var query = new GetAllUsersQuery();

        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "test1@test.com",
                Name = "Test1",
                Cpf = "11144477735",
                Password = "password",
                Birthday = new DateOnly(2000, 1, 1),
                UserType = UserType.CUSTOMER
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "test2@test.com",
                Name = "Test2",
                Cpf = "11144477735",
                Password = "password",
                Birthday = new DateOnly(2000, 1, 1),
                UserType = UserType.ADMIN
            }
        };

        var userDtos = new List<UserDto>
        {
            new UserDto 
                { 
                    Id = users[0].Id, 
                    Email = users[0].Email, 
                    Name = users[0].Name,
                    Cpf = users[0].Cpf,
                    Password = users[0].Password,
                    Birthday = users[0].Birthday,
                    UserType = (int)users[0].UserType
                },
            new UserDto 
                { 
                    Id = users[1].Id, 
                    Email = users[1].Email, 
                    Name = users[1].Name,
                    Cpf = users[1].Cpf,
                    Password = users[1].Password,
                    Birthday = users[1].Birthday,
                    UserType = (int)users[1].UserType
                }
        };

        _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);
        _mapperMock.Setup(m => m.Map<List<UserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(userDtos);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsers()
    {
        // Arrange
        var query = new GetAllUsersQuery();

        _repositoryMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());
        _mapperMock.Setup(m => m.Map<List<UserDto>>(It.IsAny<List<User>>())).Returns(new List<UserDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}