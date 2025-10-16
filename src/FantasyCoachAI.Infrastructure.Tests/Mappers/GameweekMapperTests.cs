using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Infrastructure.Mappers;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;
using FluentAssertions;
using Xunit;

namespace FantasyCoachAI.Infrastructure.Tests.Mappers
{
    public class GameweekMapperTests
    {
        [Fact]
        public void ToDomain_WhenMappingFromDbModel_ShouldMapAllProperties()
        {
            // Arrange
            var startDate = new DateTime(2025, 10, 27);
            var endDate = new DateTime(2025, 10, 29);
            var dbModel = new GameweekDbModel
            {
                Id = 1,
                Number = 16,
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var domainEntity = dbModel.ToDomain();

            // Assert
            domainEntity.Should().NotBeNull();
            domainEntity.Id.Should().Be(dbModel.Id);
            domainEntity.Number.Should().Be(dbModel.Number);
            domainEntity.StartDate.Should().Be(dbModel.StartDate);
            domainEntity.EndDate.Should().Be(dbModel.EndDate);
        }

        [Fact]
        public void ToDomain_WhenMappingFromDbModelWithDifferentDates_ShouldMapCorrectly()
        {
            // Arrange
            var startDate = new DateTime(2026, 1, 15, 14, 30, 0);
            var endDate = new DateTime(2026, 1, 17, 18, 45, 0);
            var dbModel = new GameweekDbModel
            {
                Id = 20,
                Number = 25,
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var domainEntity = dbModel.ToDomain();

            // Assert
            domainEntity.Should().NotBeNull();
            domainEntity.Id.Should().Be(20);
            domainEntity.Number.Should().Be(25);
            domainEntity.StartDate.Should().Be(startDate);
            domainEntity.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void ToDbModel_WhenMappingFromDomainEntity_ShouldMapAllProperties()
        {
            // Arrange
            var startDate = new DateTime(2025, 11, 2);
            var endDate = new DateTime(2025, 11, 4);
            var domainEntity = new Gameweek
            {
                Id = 2,
                Number = 17,
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var dbModel = domainEntity.ToDbModel();

            // Assert
            dbModel.Should().NotBeNull();
            dbModel.Id.Should().Be(domainEntity.Id);
            dbModel.Number.Should().Be(domainEntity.Number);
            dbModel.StartDate.Should().Be(domainEntity.StartDate);
            dbModel.EndDate.Should().Be(domainEntity.EndDate);
        }

        [Fact]
        public void ToDbModel_WhenMappingFromDomainEntityWithZeroId_ShouldMapCorrectly()
        {
            // Arrange - new entity without ID
            var startDate = new DateTime(2025, 12, 6);
            var endDate = new DateTime(2025, 12, 8);
            var domainEntity = new Gameweek
            {
                Id = 0, // New entity
                Number = 18,
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var dbModel = domainEntity.ToDbModel();

            // Assert
            dbModel.Should().NotBeNull();
            dbModel.Id.Should().Be(0);
            dbModel.Number.Should().Be(18);
            dbModel.StartDate.Should().Be(startDate);
            dbModel.EndDate.Should().Be(endDate);
        }

        [Fact]
        public void MapperRoundTrip_WhenMappingBackAndForth_ShouldPreserveData()
        {
            // Arrange
            var startDate = new DateTime(2025, 9, 13, 10, 0, 0);
            var endDate = new DateTime(2025, 9, 15, 22, 30, 0);
            var originalDomainEntity = new Gameweek
            {
                Id = 15,
                Number = 8,
                StartDate = startDate,
                EndDate = endDate
            };

            // Act
            var dbModel = originalDomainEntity.ToDbModel();
            var mappedBackDomainEntity = dbModel.ToDomain();

            // Assert
            mappedBackDomainEntity.Should().BeEquivalentTo(originalDomainEntity);
        }

        [Fact]
        public void MapperRoundTrip_WhenMappingMinimumValues_ShouldPreserveData()
        {
            // Arrange
            var originalDomainEntity = new Gameweek
            {
                Id = 0,
                Number = 1,
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MinValue.AddDays(1)
            };

            // Act
            var dbModel = originalDomainEntity.ToDbModel();
            var mappedBackDomainEntity = dbModel.ToDomain();

            // Assert
            mappedBackDomainEntity.Should().BeEquivalentTo(originalDomainEntity);
        }

        [Fact]
        public void MapperRoundTrip_WhenMappingMaximumValues_ShouldPreserveData()
        {
            // Arrange - Use reasonable max dates to avoid potential issues
            var maxStartDate = new DateTime(2099, 12, 30);
            var maxEndDate = new DateTime(2099, 12, 31);
            var originalDomainEntity = new Gameweek
            {
                Id = int.MaxValue,
                Number = 999,
                StartDate = maxStartDate,
                EndDate = maxEndDate
            };

            // Act
            var dbModel = originalDomainEntity.ToDbModel();
            var mappedBackDomainEntity = dbModel.ToDomain();

            // Assert
            mappedBackDomainEntity.Should().BeEquivalentTo(originalDomainEntity);
        }
    }
}
