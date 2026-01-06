using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Interfaces;
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
}