using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Exceptions;
using FantasyCoachAI.Web.Controllers;
using FluentAssertions;
using Moq;
using Xunit;

namespace FantasyCoachAI.Web.Tests.Controllers
{
    [Trait("Category", "Unit")]
    public class GameweeksControllerTests
    {
        private readonly Mock<IGameweekService> _mockGameweekService;
        private readonly GameweeksController _controller;

        public GameweeksControllerTests()
        {
            _mockGameweekService = new Mock<IGameweekService>();
            _controller = new GameweeksController(_mockGameweekService.Object);
        }

        #region GetGameweeks Tests

        [Fact]
        public async Task GetGameweeks_WhenServiceReturnsGameweeks_ShouldReturnOkWithData()
        {
            // Arrange
            var gameweeks = new List<GameweekDto>
            {
                new() { Id = 1, Number = 1, Status = GameweekStatus.Upcoming },
                new() { Id = 2, Number = 2, Status = GameweekStatus.Current }
            };

            _mockGameweekService.Setup(x => x.GetGameweeksAsync(It.IsAny<GameweekFilterDto>()))
                .ReturnsAsync(gameweeks);

            // Act
            var result = await _controller.GetGameweeks();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<dynamic>().Subject;

            ((IEnumerable<dynamic>)response.data).Should().HaveCount(2);
            ((int)response.total).Should().Be(2);
        }

        [Fact]
        public async Task GetGameweeks_WhenServiceReturnsEmptyList_ShouldReturnOkWithEmptyData()
        {
            // Arrange
            var gameweeks = new List<GameweekDto>();
            _mockGameweekService.Setup(x => x.GetGameweeksAsync(It.IsAny<GameweekFilterDto>()))
                .ReturnsAsync(gameweeks);

            // Act
            var result = await _controller.GetGameweeks();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<dynamic>().Subject;

            ((IEnumerable<dynamic>)response.data).Should().BeEmpty();
            ((int)response.total).Should().Be(0);
        }

        [Fact]
        public async Task GetGameweeks_WhenStatusFilterProvided_ShouldParseAndPassToService()
        {
            // Arrange
            var gameweeks = new List<GameweekDto>();
            _mockGameweekService.Setup(x => x.GetGameweeksAsync(It.Is<GameweekFilterDto>(
                f => f.Status == GameweekStatus.Current &&
                     f.Sort == "start_date" &&
                     f.Order == "desc")))
                .ReturnsAsync(gameweeks);

            // Act
            await _controller.GetGameweeks(status: "Current", sort: "start_date", order: "desc");

            // Assert
            _mockGameweekService.Verify(x => x.GetGameweeksAsync(It.Is<GameweekFilterDto>(
                f => f.Status == GameweekStatus.Current &&
                     f.Sort == "start_date" &&
                     f.Order == "desc")), Times.Once);
        }

        [Fact]
        public async Task GetGameweeks_WhenInvalidStatusProvided_ShouldPassNullStatus()
        {
            // Arrange
            var gameweeks = new List<GameweekDto>();
            _mockGameweekService.Setup(x => x.GetGameweeksAsync(It.Is<GameweekFilterDto>(
                f => f.Status == null)))
                .ReturnsAsync(gameweeks);

            // Act
            await _controller.GetGameweeks(status: "InvalidStatus");

            // Assert
            _mockGameweekService.Verify(x => x.GetGameweeksAsync(It.Is<GameweekFilterDto>(
                f => f.Status == null)), Times.Once);
        }

        [Fact]
        public async Task GetGameweeks_WhenNoStatusProvided_ShouldPassNullStatus()
        {
            // Arrange
            var gameweeks = new List<GameweekDto>();
            _mockGameweekService.Setup(x => x.GetGameweeksAsync(It.Is<GameweekFilterDto>(
                f => f.Status == null)))
                .ReturnsAsync(gameweeks);

            // Act
            await _controller.GetGameweeks();

            // Assert
            _mockGameweekService.Verify(x => x.GetGameweeksAsync(It.Is<GameweekFilterDto>(
                f => f.Status == null)), Times.Once);
        }

        #endregion

        #region GetGameweek Tests

        [Fact]
        public async Task GetGameweek_WhenGameweekExists_ShouldReturnOkWithGameweek()
        {
            // Arrange
            var gameweekId = 1;
            var gameweek = new GameweekDto { Id = gameweekId, Number = 1, Status = GameweekStatus.Current };

            _mockGameweekService.Setup(x => x.GetByIdAsync(gameweekId))
                .ReturnsAsync(gameweek);

            // Act
            var result = await _controller.GetGameweek(gameweekId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(gameweek);
        }

        [Fact]
        public async Task GetGameweek_WhenGameweekNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var gameweekId = 999;
            _mockGameweekService.Setup(x => x.GetByIdAsync(gameweekId))
                .ReturnsAsync((GameweekDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetGameweek(gameweekId));
            exception.Message.Should().Contain("Gameweek not found");
        }

        #endregion

        #region CreateGameweek Tests

        [Fact]
        public async Task CreateGameweek_WhenCommandIsValid_ShouldReturnCreatedWithGameweek()
        {
            // Arrange
            var createCommand = new CreateGameweekCommand { Number = 1 };
            var createdGameweek = new GameweekDto { Id = 1, Number = 1, Status = GameweekStatus.Upcoming };

            _mockGameweekService.Setup(x => x.CreateAsync(createCommand))
                .ReturnsAsync(createdGameweek);

            // Act
            var result = await _controller.CreateGameweek(createCommand);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetGameweek));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(1);
            createdResult.Value.Should().BeEquivalentTo(createdGameweek);
        }

        #endregion

        #region DeleteGameweek Tests

        [Fact]
        public async Task DeleteGameweek_WhenCalled_ShouldCallServiceAndReturnNoContent()
        {
            // Arrange
            var gameweekId = 1;
            _mockGameweekService.Setup(x => x.DeleteAsync(gameweekId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteGameweek(gameweekId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockGameweekService.Verify(x => x.DeleteAsync(gameweekId), Times.Once);
        }

        #endregion
    }
}
