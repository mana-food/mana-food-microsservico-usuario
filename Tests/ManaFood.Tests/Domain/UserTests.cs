using System;
using FluentAssertions;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Xunit;

namespace ManaFood.Tests.Domain;

public class UserTests
{
    [Fact]
    public void User_Should_Set_Required_Properties()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            Cpf = "11144477735",
            Password = "password123",
            Birthday = new DateOnly(2000, 1, 1),
            UserType = UserType.ADMIN
        };

        user.Email.Should().Be("test@test.com");
        user.Name.Should().Be("Test User");
        user.Cpf.Should().Be("11144477735");
        user.Password.Should().Be("password123");
        user.Birthday.Should().Be(new DateOnly(2000, 1, 1));
        user.UserType.Should().Be(UserType.ADMIN);
    }
}
