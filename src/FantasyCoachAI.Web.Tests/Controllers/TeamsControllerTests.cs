using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Exceptions;
using FantasyCoachAI.Web.Controllers;
using FluentAssertions;
using Moq;
using Xunit;

namespace FantasyCoachAI.Web.Tests.Controllers
{
    [Trait("Category", "Unit")]
    public class TeamsControllerTests
    {
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly TeamsController _controller;

        public TeamsControllerTests()
        {
            _mockTeamService = new Mock<ITeamService>();
            _controller = new TeamsController(_mockTeamService.Object);
        }

        #region GetTeams Tests

        [Fact]
        public async Task GetTeams_WhenServiceReturnsTeams_ShouldReturnOkWithData()
        {
            // Arrange
            var teams = new List<TeamDto>
            {
                new() { Id = 1, Name = "Team A", ShortCode = "TMA", IsActive = true },
                new() { Id = 2, Name = "Team B", ShortCode = "TMB", IsActive = false }
            };

            _mockTeamService.Setup(x => x.GetTeamsAsync(It.IsAny<TeamFilterDto>()))
                .ReturnsAsync(teams);

            // Act
            var result = await _controller.GetTeams();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<dynamic>().Subject;

            ((IEnumerable<dynamic>)response.data).Should().HaveCount(2);
            ((int)response.total).Should().Be(2);
        }

        [Fact]
        public async Task GetTeams_WhenServiceReturnsEmptyList_ShouldReturnOkWithEmptyData()
        {
            // Arrange
            var teams = new List<TeamDto>();
            _mockTeamService.Setup(x => x.GetTeamsAsync(It.IsAny<TeamFilterDto>()))
                .ReturnsAsync(teams);

            // Act
            var result = await _controller.GetTeams();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<dynamic>().Subject;

            ((IEnumerable<dynamic>)response.data).Should().BeEmpty();
            ((int)response.total).Should().Be(0);
        }

        [Fact]
        public async Task GetTeams_WhenFilterParametersProvided_ShouldPassFilterToService()
        {
            // Arrange
            var teams = new List<TeamDto>();
            _mockTeamService.Setup(x => x.GetTeamsAsync(It.Is<TeamFilterDto>(
                f => f.Sort == "name" &&
                     f.Order == "desc" &&
                     f.IsActive == true &&
                     f.ShortCode == "POL")))
                .ReturnsAsync(teams);

            // Act
            await _controller.GetTeams(sort: "name", order: "desc", isActive: true, shortCode: "POL");

            // Assert
            _mockTeamService.Verify(x => x.GetTeamsAsync(It.Is<TeamFilterDto>(
                f => f.Sort == "name" &&
                     f.Order == "desc" &&
                     f.IsActive == true &&
                     f.ShortCode == "POL")), Times.Once);
        }

        #endregion

        #region GetTeam Tests

        [Fact]
        public async Task GetTeam_WhenIdIsValidAndTeamExists_ShouldReturnOkWithTeam()
        {
            // Arrange
            var teamId = 1;
            var team = new TeamDto { Id = teamId, Name = "Test Team", ShortCode = "TST", IsActive = true };

            _mockTeamService.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);

            // Act
            var result = await _controller.GetTeam(teamId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(team);
        }

        [Fact]
        public async Task GetTeam_WhenIdIsValidButTeamNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var teamId = 999;
            _mockTeamService.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((TeamDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetTeam(teamId));
            exception.Message.Should().Contain("Team not found");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetTeam_WhenIdIsInvalid_ShouldThrowArgumentException(int invalidId)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.GetTeam(invalidId));
            exception.Message.Should().Contain("Invalid team ID");
        }

        #endregion

        #region CreateTeam Tests

        [Fact]
        public async Task CreateTeam_WhenCommandIsValid_ShouldReturnCreatedWithTeam()
        {
            // Arrange
            var createCommand = new CreateTeamCommand { Name = "New Team", ShortCode = "NEW" };
            var createdTeam = new TeamDto { Id = 1, Name = "New Team", ShortCode = "NEW", IsActive = true };

            _mockTeamService.Setup(x => x.CreateAsync(createCommand))
                .ReturnsAsync(createdTeam);

            // Act
            var result = await _controller.CreateTeam(createCommand);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetTeam));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(1);
            createdResult.Value.Should().BeEquivalentTo(createdTeam);
        }

        #endregion

        #region UpdateTeam Tests

        [Fact]
        public async Task UpdateTeam_WhenCommandIsValid_ShouldReturnOkWithUpdatedTeam()
        {
            // Arrange
            var teamId = 1;
            var updateCommand = new UpdateTeamCommand { IsActive = false };
            var updatedTeam = new TeamDto { Id = teamId, Name = "Updated Team", ShortCode = "UPD", IsActive = false };

            _mockTeamService.Setup(x => x.UpdateAsync(It.Is<UpdateTeamCommand>(c => c.Id == teamId)))
                .ReturnsAsync(updatedTeam);

            // Act
            var result = await _controller.UpdateTeam(teamId, updateCommand);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(updatedTeam);
            updateCommand.Id.Should().Be(teamId);
        }

        [Fact]
        public async Task UpdateTeam_ShouldSetCommandIdFromRouteParameter()
        {
            // Arrange
            var routeId = 5;
            var updateCommand = new UpdateTeamCommand { IsActive = true };
            var updatedTeam = new TeamDto { Id = routeId, Name = "Updated Team", IsActive = true };

            _mockTeamService.Setup(x => x.UpdateAsync(It.Is<UpdateTeamCommand>(c => c.Id == routeId)))
                .ReturnsAsync(updatedTeam);

            // Act
            await _controller.UpdateTeam(routeId, updateCommand);

            // Assert
            updateCommand.Id.Should().Be(routeId);
        }

        #endregion
    }
}
