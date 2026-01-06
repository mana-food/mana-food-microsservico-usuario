using FluentAssertions;
using ManaFood.Application.Utils;

namespace ManaFood.Tests.Utils;

public class CpfUtilsTests
{
    [Theory]
    [InlineData("11144477735", true)]
    [InlineData("11144477735", true)]
    [InlineData("00000000000", false)]
    [InlineData("11111111111", false)]
    [InlineData("123456789", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("12345678900", false)]
    public void IsValidCpf_ShouldValidateCorrectly(string cpf, bool expected)
    {
        // Act
        var result = CpfUtils.IsValidCpf(cpf);

        // Assert
        result.Should().Be(expected);
    }
}