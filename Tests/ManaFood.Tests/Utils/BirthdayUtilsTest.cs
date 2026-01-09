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

    [Theory]
    [InlineData(-18, true)]   // Exatamente 18 anos
    [InlineData(-19, true)]   // 19 anos
    [InlineData(-30, true)]   // 30 anos
    [InlineData(-50, true)]   // 50 anos
    [InlineData(-100, true)]  // 100 anos
    [InlineData(-17, false)]  // 17 anos
    [InlineData(-16, false)]  // 16 anos
    [InlineData(-10, false)]  // 10 anos
    [InlineData(-5, false)]   // 5 anos
    [InlineData(0, false)]    // Nascido hoje
    public void IsValidBirthday_ShouldValidateCorrectly_ForDifferentAges(int yearsToSubtract, bool expected)
    {
        // Arrange
        var birthday = DateOnly.FromDateTime(DateTime.Today.AddYears(yearsToSubtract));

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsValidBirthday_ShouldReturnTrue_When18YearsOldExactly()
    {
        // Arrange - Pessoa que faz 18 anos hoje
        var birthday = new DateOnly(DateTime.Today.Year - 18, DateTime.Today.Month, DateTime.Today.Day);

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidBirthday_ShouldReturnFalse_WhenBirthdayIsTomorrow18YearsAgo()
    {
        // Arrange - Pessoa que vai fazer 18 anos amanhã
        var tomorrow = DateTime.Today.AddDays(1);
        var birthday = new DateOnly(tomorrow.Year - 18, tomorrow.Month, tomorrow.Day);

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidBirthday_ShouldReturnTrue_WhenBirthdayWasYesterday18YearsAgo()
    {
        // Arrange - Pessoa que fez 18 anos ontem
        var yesterday = DateTime.Today.AddDays(-1);
        var birthday = new DateOnly(yesterday.Year - 18, yesterday.Month, yesterday.Day);

        // Act
        var result = BirthdayUtils.IsValidBirthday(birthday);

        // Assert
        result.Should().BeTrue();
    }
}