using FantasyCoachAI.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace FantasyCoachAI.Domain.Tests.Entities
{
    public class TeamTests
    {
        [Fact]
        public void HasCrest_WhenCrestUrlIsNotEmpty_ShouldReturnTrue()
        {
            // Arrange
            var team = new Team
            {
                Id = 1,
                Name = "Test Team",
                CrestUrl = "https://example.com/crest.png"
            };

            // Act
            var result = team.HasCrest();

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void HasCrest_WhenCrestUrlIsNullOrEmpty_ShouldReturnFalse(string? crestUrl)
        {
            // Arrange
            var team = new Team
            {
                Id = 1,
                Name = "Test Team",
                CrestUrl = crestUrl
            };

            // Act
            var result = team.HasCrest();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Team_ShouldInitializeWithDefaultValues()
        {
            // Act
            var team = new Team();

            // Assert
            team.Id.Should().Be(0);
            team.Name.Should().Be(string.Empty);
            team.ShortCode.Should().BeNull();
            team.CrestUrl.Should().BeNull();
            team.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Team_ShouldSetPropertiesCorrectly()
        {
            // Arrange & Act
            var team = new Team
            {
                Id = 1,
                Name = "Manchester United",
                ShortCode = "MUN",
                CrestUrl = "https://example.com/crest.png",
                IsActive = true
            };

            // Assert
            team.Id.Should().Be(1);
            team.Name.Should().Be("Manchester United");
            team.ShortCode.Should().Be("MUN");
            team.CrestUrl.Should().Be("https://example.com/crest.png");
            team.IsActive.Should().BeTrue();
        }
    }
}
