using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.UseCases.UserUseCase.Commands.CreateUser;
using ManaFood.Application.UseCases.UserUseCase.Commands.DeleteUser;
using ManaFood.Application.UseCases.UserUseCase.Commands.UpdateUser;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetAllUsers;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByCpf;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserByEmail;
using ManaFood.Application.UseCases.UserUseCase.Queries.GetUserById;
using ManaFood.Domain.Enums;
using ManaFood.WebAPI.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ManaFood.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new UserController(_mediatorMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithListOfUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@test.com", Cpf = "11144477735", Password = "senha1", Birthday = new DateOnly(2000, 1, 1), UserType = (int)UserType.CUSTOMER },
            new UserDto { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@test.com", Cpf = "22255588899", Password = "senha2", Birthday = new DateOnly(2000, 1, 1), UserType = (int)UserType.CUSTOMER }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUsers = okResult.Value.Should().BeAssignableTo<List<UserDto>>().Subject;
        returnedUsers.Should().HaveCount(2);
        returnedUsers.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task GetAll_ShouldCallMediatorWithCorrectQuery()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDto>());

        // Act
        await _controller.GetAll(CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserDto
        {
            Id = userId,
            Name = "Test User",
            Email = "test@test.com",
            Cpf = "11144477735",
            Password = "senha",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = (int)UserType.CUSTOMER
        };

        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByIdQuery>(q => q.Id == userId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(userId, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeAssignableTo<UserDto>().Subject;
        returnedUser.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetById_ShouldCallMediatorWithCorrectId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto {
                Id = userId,
                Name = "Test User",
                Email = "test@test.com",
                Cpf = "11144477735",
                Password = "senha",
                Birthday = new DateOnly(2000, 1, 1),
                UserType = (int)UserType.CUSTOMER
            });

        // Act
        await _controller.GetById(userId, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<GetUserByIdQuery>(q => q.Id == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByEmail Tests

    [Fact]
    public async Task GetByEmail_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        var email = "test@test.com";
        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            Cpf = "11144477735",
            Password = "senha",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = (int)UserType.CUSTOMER
        };

        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByEmailQuery>(q => q.Email == email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetByEmail(email, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeAssignableTo<UserDto>().Subject;
        returnedUser.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmail_ShouldCallMediatorWithCorrectEmail()
    {
        // Arrange
        var email = "test@test.com";
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = email,
                Cpf = "11144477735",
                Password = "senha",
                Birthday = new DateOnly(2000, 1, 1),
                UserType = (int)UserType.CUSTOMER
            });

        // Act
        await _controller.GetByEmail(email, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<GetUserByEmailQuery>(q => q.Email == email),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByCpf Tests

    [Fact]
    public async Task GetByCpf_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        var cpf = "11144477735";
        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@test.com",
            Cpf = cpf,
            Password = "senha",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = (int)UserType.CUSTOMER
        };

        _mediatorMock.Setup(m => m.Send(It.Is<GetUserByCpfQuery>(q => q.Cpf == cpf), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetByCpf(cpf, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeAssignableTo<UserDto>().Subject;
        returnedUser.Cpf.Should().Be(cpf);
    }

    [Fact]
    public async Task GetByCpf_ShouldCallMediatorWithCorrectCpf()
    {
        // Arrange
        var cpf = "11144477735";
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByCpfQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@test.com",
                Cpf = cpf,
                Password = "senha",
                Birthday = new DateOnly(2000, 1, 1),
                UserType = (int)UserType.CUSTOMER
            });

        // Act
        await _controller.GetByCpf(cpf, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<GetUserByCpfQuery>(q => q.Cpf == cpf),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ShouldReturnCreatedAtActionWithUser()
    {
        // Arrange
        var command = new CreateUserCommand(
            "newuser@test.com",
            "New User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            (int)UserType.CUSTOMER
        );

        var createdUser = new UserDto
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = command.UserType
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.Create(command, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(UserController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(createdUser.Id);
        var returnedUser = createdResult.Value.Should().BeAssignableTo<UserDto>().Subject;
        returnedUser.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task Create_ShouldCallMediatorWithCorrectCommand()
    {
        // Arrange
        var command = new CreateUserCommand(
            "newuser@test.com",
            "New User",
            "11144477735",
            "password123",
            new DateOnly(2000, 1, 1),
            (int)UserType.CUSTOMER
        );

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Email = command.Email,
                Cpf = command.Cpf,
                Password = command.Password,
                Birthday = command.Birthday,
                UserType = command.UserType
            });

        // Act
        await _controller.Create(command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<CreateUserCommand>(c => c.Email == command.Email && c.Cpf == command.Cpf),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ShouldReturnOkWithUpdatedUser_WhenIdsMatch()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(
            userId,
            "updated@test.com",
            "Updated User",
            "11144477735",
            "newpassword",
            new DateOnly(2000, 1, 1),
            (int)UserType.ADMIN
        );

        var updatedUser = new UserDto
        {
            Id = userId,
            Name = command.Name,
            Email = command.Email,
            Cpf = command.Cpf,
            Password = command.Password,
            Birthday = command.Birthday,
            UserType = command.UserType
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.Update(userId, command, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedUser = okResult.Value.Should().BeAssignableTo<UserDto>().Subject;
        returnedUser.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdsDontMatch()
    {
        // Arrange
        var urlId = Guid.NewGuid();
        var commandId = Guid.NewGuid();
        var command = new UpdateUserCommand(
            commandId,
            "Updated User",
            "updated@test.com",
            "11144477735",
            "newpassword",
            new DateOnly(2000, 1, 1),
            (int)UserType.CUSTOMER
        );

        // Act
        var result = await _controller.Update(urlId, command, CancellationToken.None);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Incompatibilidade de ID entre URL e corpo da solicitação");
    }

    [Fact]
    public async Task Update_ShouldNotCallMediator_WhenIdsDontMatch()
    {
        // Arrange
        var urlId = Guid.NewGuid();
        var commandId = Guid.NewGuid();
        var command = new UpdateUserCommand(
            commandId,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            new DateOnly(2000, 1, 1),
            0
        );

        // Act
        await _controller.Update(urlId, command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldCallMediatorWithCorrectCommand_WhenIdsMatch()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand(
            userId,
            "updated@test.com",
            "Updated User",
            string.Empty,
            string.Empty,
            new DateOnly(2000, 1, 1),
            0
        );

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto {
                Id = userId,
                Name = command.Name,
                Email = command.Email,
                Cpf = command.Cpf,
                Password = command.Password,
                Birthday = command.Birthday,
                UserType = command.UserType
            });

        // Act
        await _controller.Update(userId, command, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(
            It.Is<UpdateUserCommand>(c => c.Id == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenIdsMatch()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.Delete(userId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenIdsDontMatch()
    {
        // Arrange
        var urlId = Guid.NewGuid();
        var commandId = Guid.NewGuid();
        var command = new DeleteUserCommand(commandId);

        // Act
        var result = await _controller.Delete(urlId, command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}
