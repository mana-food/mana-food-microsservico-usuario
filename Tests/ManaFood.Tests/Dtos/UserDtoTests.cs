using System;
using FluentAssertions;
using ManaFood.Application.Dtos;
using Xunit;

namespace ManaFood.Tests.Dtos;

public class UserDtoTests
{
    [Fact]
    public void UserDto_Should_Set_Properties_Correctly()
    {
        var dto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password123",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = 1
        };

        dto.Email.Should().Be("test@test.com");
        dto.Name.Should().Be("Test User");
        dto.Cpf.Should().Be("11144477735");
        dto.Password.Should().Be("password123");
        dto.Birthday.Should().Be(new DateOnly(2000, 1, 1));
        dto.UserType.Should().Be(1);
    }

    [Fact]
    public void UserDto_Equality_Should_Work()
    {
        var id = Guid.NewGuid();
        var dto1 = new UserDto
        {
            Id = id,
            Email = "test@test.com",
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password123",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = 1
        };
        var dto2 = new UserDto
        {
            Id = id,
            Email = "test@test.com",
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password123",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = 1
        };
        dto1.Should().Be(dto2);
    }
}
