using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Infrastructure.Mappers;
using FantasyCoachAI.Infrastructure.Persistence.SupabaseModels;
using FluentAssertions;
using Xunit;

namespace FantasyCoachAI.Infrastructure.Tests.Mappers
{
    [Trait("Category", "Unit")]
    public class TeamMapperTests
    {
        [Fact]
        public void ToDomain_WhenMappingFromDbModel_ShouldMapAllProperties()
        {
            // Arrange
            var dbModel = new TeamDbModel
            {
                Id = 1,
                Name = "Manchester United",
                ShortCode = "MAN",
                CrestUrl = "https://example.com/crest.png",
                IsActive = true
            };

            // Act
            var domainEntity = dbModel.ToDomain();

            // Assert
            domainEntity.Should().NotBeNull();
            domainEntity.Id.Should().Be(dbModel.Id);
            domainEntity.Name.Should().Be(dbModel.Name);
            domainEntity.ShortCode.Should().Be(dbModel.ShortCode);
            domainEntity.CrestUrl.Should().Be(dbModel.CrestUrl);
            domainEntity.IsActive.Should().Be(dbModel.IsActive);
        }

        [Fact]
        public void ToDomain_WhenMappingFromDbModelWithNulls_ShouldMapCorrectly()
        {
            // Arrange
            var dbModel = new TeamDbModel
            {
                Id = 2,
                Name = "Test Team",
                ShortCode = null,
                CrestUrl = null,
                IsActive = false
            };

            // Act
            var domainEntity = dbModel.ToDomain();

            // Assert
            domainEntity.Should().NotBeNull();
            domainEntity.Id.Should().Be(2);
            domainEntity.Name.Should().Be("Test Team");
            domainEntity.ShortCode.Should().BeNull();
            domainEntity.CrestUrl.Should().BeNull();
            domainEntity.IsActive.Should().BeFalse();
        }

        [Fact]
        public void ToDbModel_WhenMappingFromDomainEntity_ShouldMapAllProperties()
        {
            // Arrange
            var domainEntity = new Team
            {
                Id = 1,
                Name = "Chelsea FC",
                ShortCode = "CHE",
                CrestUrl = "https://example.com/chelsea-crest.png",
                IsActive = true
            };

            // Act
            var dbModel = domainEntity.ToDbModel();

            // Assert
            dbModel.Should().NotBeNull();
            dbModel.Id.Should().Be(domainEntity.Id);
            dbModel.Name.Should().Be(domainEntity.Name);
            dbModel.ShortCode.Should().Be(domainEntity.ShortCode);
            dbModel.CrestUrl.Should().Be(domainEntity.CrestUrl);
            dbModel.IsActive.Should().Be(domainEntity.IsActive);
        }

        [Fact]
        public void ToDbModel_WhenMappingFromDomainEntityWithNulls_ShouldMapCorrectly()
        {
            // Arrange
            var domainEntity = new Team
            {
                Id = 3,
                Name = "Test Team",
                ShortCode = null,
                CrestUrl = null,
                IsActive = false
            };

            // Act
            var dbModel = domainEntity.ToDbModel();

            // Assert
            dbModel.Should().NotBeNull();
            dbModel.Id.Should().Be(3);
            dbModel.Name.Should().Be("Test Team");
            dbModel.ShortCode.Should().BeNull();
            dbModel.CrestUrl.Should().BeNull();
            dbModel.IsActive.Should().BeFalse();
        }

        [Fact]
        public void ToInsertDbModel_WhenMappingFromDomainEntity_ShouldMapAllPropertiesExceptId()
        {
            // Arrange
            var domainEntity = new Team
            {
                Id = 1, // This should be ignored for inserts
                Name = "Newcastle United",
                ShortCode = "NEW",
                CrestUrl = "https://example.com/newcastle-crest.png",
                IsActive = true
            };

            // Act
            var insertDbModel = domainEntity.ToInsertDbModel();

            // Assert
            insertDbModel.Should().NotBeNull();
            insertDbModel.Name.Should().Be(domainEntity.Name);
            insertDbModel.ShortCode.Should().Be(domainEntity.ShortCode);
            insertDbModel.CrestUrl.Should().Be(domainEntity.CrestUrl);
            insertDbModel.IsActive.Should().Be(domainEntity.IsActive);
        }

        [Fact]
        public void ToInsertDbModel_WhenMappingFromDomainEntityWithNulls_ShouldMapCorrectly()
        {
            // Arrange
            var domainEntity = new Team
            {
                Id = 10, // This should be ignored for inserts
                Name = "Insert Test Team",
                ShortCode = null,
                CrestUrl = null,
                IsActive = false
            };

            // Act
            var insertDbModel = domainEntity.ToInsertDbModel();

            // Assert
            insertDbModel.Should().NotBeNull();
            insertDbModel.Name.Should().Be("Insert Test Team");
            insertDbModel.ShortCode.Should().BeNull();
            insertDbModel.CrestUrl.Should().BeNull();
            insertDbModel.IsActive.Should().BeFalse();
        }

        [Fact]
        public void MapperRoundTrip_WhenMappingBackAndForth_ShouldPreserveData()
        {
            // Arrange
            var originalDomainEntity = new Team
            {
                Id = 5,
                Name = "Arsenal FC",
                ShortCode = "ARS",
                CrestUrl = "https://example.com/arsenal.png",
                IsActive = true
            };

            // Act
            var dbModel = originalDomainEntity.ToDbModel();
            var mappedBackDomainEntity = dbModel.ToDomain();

            // Assert
            mappedBackDomainEntity.Should().BeEquivalentTo(originalDomainEntity);
        }
    }
}
