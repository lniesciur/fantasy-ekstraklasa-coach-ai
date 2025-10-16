using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace FantasyCoachAI.Domain.Tests.Entities
{
    public class GameweekTests
    {
        [Fact]
        public void Gameweek_ShouldInitializeWithDefaultValues()
        {
            // Act
            var gameweek = new Gameweek();

            // Assert
            gameweek.Id.Should().Be(0);
            gameweek.Number.Should().Be(0);
            gameweek.StartDate.Should().Be(default);
            gameweek.EndDate.Should().Be(default);
        }

        [Fact]
        public void Gameweek_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var startDate = new DateTime(2025, 10, 27);
            var endDate = new DateTime(2025, 10, 29);
            var gameweek = new Gameweek
            {
                Id = 1,
                Number = 16,
                StartDate = startDate,
                EndDate = endDate
            };

            // Assert
            gameweek.Id.Should().Be(1);
            gameweek.Number.Should().Be(16);
            gameweek.StartDate.Should().Be(startDate);
            gameweek.EndDate.Should().Be(endDate);
        }

        #region GetStatus Tests

        [Fact]
        public void GetStatus_WhenStartDateIsInFuture_ShouldReturnUpcoming()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var status = gameweek.GetStatus();

            // Assert
            status.Should().Be(GameweekStatus.Upcoming);
        }

        [Fact]
        public void GetStatus_WhenCurrentDateIsBetweenStartAndEnd_ShouldReturnCurrent()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var status = gameweek.GetStatus();

            // Assert
            status.Should().Be(GameweekStatus.Current);
        }

        [Fact]
        public void GetStatus_WhenEndDateIsInPast_ShouldReturnCompleted()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow.AddDays(-5)
            };

            // Act
            var status = gameweek.GetStatus();

            // Assert
            status.Should().Be(GameweekStatus.Completed);
        }

        [Fact]
        public void GetStatus_WhenStartDateIsToday_ShouldReturnCurrent()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var gameweek = new Gameweek
            {
                StartDate = today,
                EndDate = today.AddDays(2)
            };

            // Act
            var status = gameweek.GetStatus();

            // Assert
            status.Should().Be(GameweekStatus.Current);
        }

        [Fact]
        public void GetStatus_WhenEndDateIsToday_ShouldReturnCurrent()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var gameweek = new Gameweek
            {
                StartDate = today.AddDays(-2),
                EndDate = today
            };

            // Act
            var status = gameweek.GetStatus();

            // Assert
            status.Should().Be(GameweekStatus.Current);
        }

        #endregion

        #region IsValidDateRange Tests

        [Fact]
        public void IsValidDateRange_WhenStartDateIsBeforeEndDate_ShouldReturnTrue()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = new DateTime(2025, 10, 27),
                EndDate = new DateTime(2025, 10, 29)
            };

            // Act
            var result = gameweek.IsValidDateRange();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidDateRange_WhenStartDateIsAfterEndDate_ShouldReturnFalse()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = new DateTime(2025, 10, 29),
                EndDate = new DateTime(2025, 10, 27)
            };

            // Act
            var result = gameweek.IsValidDateRange();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidDateRange_WhenStartDateEqualsEndDate_ShouldReturnFalse()
        {
            // Arrange
            var date = new DateTime(2025, 10, 27);
            var gameweek = new Gameweek
            {
                StartDate = date,
                EndDate = date
            };

            // Act
            var result = gameweek.IsValidDateRange();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidDateRange_WhenStartDateIsOneDayBeforeEndDate_ShouldReturnTrue()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = new DateTime(2025, 10, 27),
                EndDate = new DateTime(2025, 10, 28)
            };

            // Act
            var result = gameweek.IsValidDateRange();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region HasStarted Tests

        [Fact]
        public void HasStarted_WhenStartDateIsInPast_ShouldReturnTrue()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var result = gameweek.HasStarted();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasStarted_WhenStartDateIsInFuture_ShouldReturnFalse()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = gameweek.HasStarted();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasStarted_WhenStartDateIsToday_ShouldReturnTrue()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.Date
            };

            // Act
            var result = gameweek.HasStarted();

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region IsActive Tests

        [Fact]
        public void IsActive_WhenGameweekIsCurrent_ShouldReturnTrue()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = gameweek.IsActive();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsActive_WhenGameweekIsUpcoming_ShouldReturnFalse()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3)
            };

            // Act
            var result = gameweek.IsActive();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsActive_WhenGameweekIsCompleted_ShouldReturnFalse()
        {
            // Arrange
            var gameweek = new Gameweek
            {
                StartDate = DateTime.UtcNow.AddDays(-3),
                EndDate = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var result = gameweek.IsActive();

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
