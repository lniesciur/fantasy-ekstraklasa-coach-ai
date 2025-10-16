using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Services;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace FantasyCoachAI.Application.Tests.Services
{
    public class TeamServiceTests
    {
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly TeamService _teamService;

        public TeamServiceTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();
            _teamService = new TeamService(_mockTeamRepository.Object);
        }

        #region GetTeamsAsync Tests

        [Fact]
        public async Task GetTeamsAsync_WhenNoFilterProvided_ShouldReturnAllTeams()
        {
            // Arrange
            var expectedTeams = new List<Team>
            {
                new() { Id = 1, Name = "Team A", IsActive = true },
                new() { Id = 2, Name = "Team B", IsActive = false }
            };
            
            _mockTeamRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedTeams);

            // Act
            var result = await _teamService.GetTeamsAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedTeams);
            _mockTeamRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTeamsAsync_WhenFilterProvided_ShouldReturnFilteredTeams()
        {
            // Arrange
            var filter = new TeamFilterDto { IsActive = true, ShortCode = "MAN" };
            var expectedTeams = new List<Team>
            {
                new() { Id = 1, Name = "Manchester United", ShortCode = "MAN", IsActive = true }
            };

            _mockTeamRepository.Setup(x => x.GetFilteredAsync(filter.IsActive, filter.ShortCode))
                .ReturnsAsync(expectedTeams);

            // Act
            var result = await _teamService.GetTeamsAsync(filter);

            // Assert
            result.Should().BeEquivalentTo(expectedTeams);
            _mockTeamRepository.Verify(x => x.GetFilteredAsync(filter.IsActive, filter.ShortCode), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WhenIdIsValid_ShouldReturnTeam()
        {
            // Arrange
            var teamId = 1;
            var expectedTeam = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            
            _mockTeamRepository.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(expectedTeam);

            // Act
            var result = await _teamService.GetByIdAsync(teamId);

            // Assert
            result.Should().BeEquivalentTo(expectedTeam);
            _mockTeamRepository.Verify(x => x.GetByIdAsync(teamId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTeamNotFound_ShouldReturnNull()
        {
            // Arrange
            var teamId = 999;
            
            _mockTeamRepository.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((Team?)null);

            // Act
            var result = await _teamService.GetByIdAsync(teamId);

            // Assert
            result.Should().BeNull();
            _mockTeamRepository.Verify(x => x.GetByIdAsync(teamId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetByIdAsync_WhenIdIsInvalid_ShouldThrowArgumentException(int invalidId)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _teamService.GetByIdAsync(invalidId));
            exception.Message.Should().Contain("ID must be greater than 0");
            exception.ParamName.Should().Be("id");
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WhenTeamIsValid_ShouldCreateTeam()
        {
            // Arrange
            var newTeamCommand = new CreateTeamCommand { Name = "New Team", ShortCode = "NEW" };
            var existingTeams = new List<Team>();
            var createdTeam = new Team { Id = 1, Name = "New Team", ShortCode = "NEW", IsActive = true };

            _mockTeamRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(existingTeams);
            _mockTeamRepository.Setup(x => x.CreateAsync(It.IsAny<Team>()))
                .ReturnsAsync(createdTeam);

            // Act
            var result = await _teamService.CreateAsync(newTeamCommand);

            // Assert
            result.Should().BeEquivalentTo(createdTeam);
            result.IsActive.Should().BeTrue();
            _mockTeamRepository.Verify(x => x.GetAllAsync(), Times.Once);
            _mockTeamRepository.Verify(x => x.CreateAsync(It.Is<Team>(t => t.IsActive == true)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WhenNameIsNullOrEmpty_ShouldThrowArgumentException(string? invalidName)
        {
            // Arrange
            var newTeamCommand = new CreateTeamCommand { Name = invalidName! };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _teamService.CreateAsync(newTeamCommand));
            exception.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task CreateAsync_WhenTeamWithSameNameExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var newTeamCommand = new CreateTeamCommand { Name = "Existing Team", ShortCode = "EXT" };
            var existingTeams = new List<Team>
            {
                new() { Id = 1, Name = "existing team", ShortCode = "EXT" } // Different case to test case-insensitive comparison
            };

            _mockTeamRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(existingTeams);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _teamService.CreateAsync(newTeamCommand));
            exception.Message.Should().Contain("A team named 'Existing Team' already exists");
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WhenTeamIsValid_ShouldUpdateTeam()
        {
            // Arrange
            var teamToUpdateCommand = new UpdateTeamCommand { Id = 1, Name = "Updated Team", ShortCode = "UT", IsActive = true };
            var existingTeam = new Team { Id = 1, Name = "Original Team", ShortCode = "OT", IsActive = true };
            var allTeams = new List<Team> { existingTeam };

            _mockTeamRepository.Setup(x => x.GetByIdAsync(teamToUpdateCommand.Id))
                .ReturnsAsync(existingTeam);
            _mockTeamRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(allTeams);

            // Act
            var result = await _teamService.UpdateAsync(teamToUpdateCommand);

            // Assert
            result.Should().BeEquivalentTo(existingTeam);
            _mockTeamRepository.Verify(x => x.GetByIdAsync(teamToUpdateCommand.Id), Times.Once);
            _mockTeamRepository.Verify(x => x.UpdateAsync(existingTeam), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task UpdateAsync_WhenIdIsInvalid_ShouldThrowArgumentException(int invalidId)
        {
            // Arrange
            var teamToUpdateCommand = new UpdateTeamCommand { Id = invalidId, Name = "Test Team" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _teamService.UpdateAsync(teamToUpdateCommand));
            exception.Message.Should().Be("ID must be greater than 0");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateAsync_WhenNameIsNullOrEmpty_ShouldThrowArgumentException(string? invalidName)
        {
            // Arrange
            var teamToUpdateCommand = new UpdateTeamCommand { Id = 1, Name = invalidName! };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _teamService.UpdateAsync(teamToUpdateCommand));
            exception.Message.Should().Be("Name is required");
        }

        [Fact]
        public async Task UpdateAsync_WhenTeamNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var teamToUpdateCommand = new UpdateTeamCommand { Id = 999, Name = "Non-existent Team", ShortCode = "NET" };

            _mockTeamRepository.Setup(x => x.GetByIdAsync(teamToUpdateCommand.Id))
                .ReturnsAsync((Team?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _teamService.UpdateAsync(teamToUpdateCommand));
            exception.Message.Should().Contain("Team with ID 999 not found");
        }

        #endregion
    }
}
