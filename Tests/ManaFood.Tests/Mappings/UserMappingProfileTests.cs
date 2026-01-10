using System;
using AutoMapper;
using FluentAssertions;
using ManaFood.Application.Dtos;
using ManaFood.Application.Mappings;
using ManaFood.Domain.Entities;
using ManaFood.Domain.Enums;
using Xunit;

namespace ManaFood.Tests.Mappings;

public class UserMappingProfileTests
{
    private readonly IMapper _mapper;

    public UserMappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMapper>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void UserMapper_Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMapper>());
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Should_Map_User_To_UserDto()
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
        var dto = _mapper.Map<UserDto>(user);
        dto.Email.Should().Be(user.Email);
        dto.Name.Should().Be(user.Name);
        dto.Cpf.Should().Be(user.Cpf);
        dto.Password.Should().Be(user.Password);
        dto.Birthday.Should().Be(user.Birthday);
        dto.UserType.Should().Be((int)user.UserType);
    }
}
