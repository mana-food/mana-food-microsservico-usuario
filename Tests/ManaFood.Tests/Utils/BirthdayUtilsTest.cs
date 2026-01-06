using FluentAssertions;
using ManaFood.Application.Utils;

namespace ManaFood.Tests.Utils;

public class BirthdayUtilsTests
{
    [Fact]
    public void IsValidBirthday_ShouldReturnTrue_WhenAgeIs18OrMore()
    {
        // Arrange
        var birthday = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidBirthday_ShouldReturnTrue_WhenAgeIs25()
    {
        // Arrange
        var birthday = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidBirthday_ShouldReturnFalse_WhenAgeIsLessThan18()
    {
        // Arrange
        var birthday = DateOnly.FromDateTime(DateTime.Today.AddYears(-17));

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidBirthday_ShouldReturnFalse_WhenAgeIs10()
    {
        // Arrange
        var birthday = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeFalse();
    }
}