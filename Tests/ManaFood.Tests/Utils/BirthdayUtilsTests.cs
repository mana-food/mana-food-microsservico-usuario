using System;
using System.Threading.Tasks;
using FluentAssertions;
using ManaFood.Application.Utils;
using Xunit;

namespace ManaFood.Tests.Utils
{
    public class BirthdayUtilsTests
    {
        [Theory]
        [InlineData(2000, 1, 1, true)] // Over 18
        [InlineData(2010, 1, 1, false)] // Under 18
        [InlineData(2008, 1, 1, true)] // Exactly 18 if today is after Jan 1, 2026
        public void IsValidBirthday_ShouldReturnExpectedResult(int year, int month, int day, bool expected)
        {
            // Arrange
            var birthday = new DateOnly(year, month, day);

            // Act
            var result = BirthdayUtils.IsValidBirthday(birthday);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidBirthday_ShouldReturnFalse_ForFutureBirthday()
        {
            // Arrange
            var futureBirthday = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

            // Act
            var result = BirthdayUtils.IsValidBirthday(futureBirthday);

            // Assert
            result.Should().BeFalse();
        }
    }
}
