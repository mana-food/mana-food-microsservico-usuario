using FluentAssertions;
using ManaFood.Application.Utils;
using Xunit;

namespace ManaFood.Tests.Utils;

public class PasswordHasherTests
{
    [Fact]
    public void HashPassword_ShouldReturnConsistentHash_ForSameInput()
    {
        var password = "senha123";
        var hash1 = PasswordHasher.HashPassword(password);
        var hash2 = PasswordHasher.HashPassword(password);
        hash1.Should().Be(hash2);
        hash1.Should().NotBe(password);
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHash_ForDifferentInputs()
    {
        var hash1 = PasswordHasher.HashPassword("senha123");
        var hash2 = PasswordHasher.HashPassword("outraSenha");
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void HashPassword_ShouldNotThrow_ForEmptyString()
    {
        var hash = PasswordHasher.HashPassword("");
        hash.Should().NotBeNullOrEmpty();
    }
}
