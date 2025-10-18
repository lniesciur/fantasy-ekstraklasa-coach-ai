using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Application.Interfaces;
using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Web.Controllers;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace FantasyCoachAI.Web.Tests.Controllers
{
    [Trait("Category", "Unit")]
    public class PlayerStatsControllerTests
    {
        private readonly Mock<IPlayerStatsService> _mockPlayerStatsService;
        private readonly PlayerStatsController _controller;

        public PlayerStatsControllerTests()
        {
            _mockPlayerStatsService = new Mock<IPlayerStatsService>();
            _controller = new PlayerStatsController(_mockPlayerStatsService.Object);
        }

        #region GetPlayerStats Tests

        [Fact]
        public async Task GetPlayerStats_WhenServiceReturnsData_ShouldReturnOkWithResponse()
        {
            // Arrange
            var filter = new PlayerStatsFilterDto { Page = 1, Limit = 10 };
            var stats = new List<PlayerStatsDto>
            {
                new() { Id = 1, Player = new() { Id = 1, Name = "Player 1", Position = "GK" }, FantasyPoints = 10 },
                new() { Id = 2, Player = new() { Id = 2, Name = "Player 2", Position = "DEF" }, FantasyPoints = 8 }
            };
            var total = 25;

            _mockPlayerStatsService.Setup(x => x.GetStatsAsync(filter))
                .ReturnsAsync((stats, total));

            // Act
            var result = await _controller.GetPlayerStats(filter);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<PlayerStatsResponseDto>().Subject;

            response.Data.Should().BeEquivalentTo(stats);
            response.Pagination.Page.Should().Be(1);
            response.Pagination.Limit.Should().Be(10);
            response.Pagination.Total.Should().Be(25);
            response.Pagination.Pages.Should().Be(3); // 25 / 10 = 2.5 -> 3 pages
        }

        [Fact]
        public async Task GetPlayerStats_WhenServiceReturnsEmptyData_ShouldReturnOkWithEmptyResponse()
        {
            // Arrange
            var filter = new PlayerStatsFilterDto { Page = 1, Limit = 10 };
            var stats = new List<PlayerStatsDto>();
            var total = 0;

            _mockPlayerStatsService.Setup(x => x.GetStatsAsync(filter))
                .ReturnsAsync((stats, total));

            // Act
            var result = await _controller.GetPlayerStats(filter);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<PlayerStatsResponseDto>().Subject;

            response.Data.Should().BeEmpty();
            response.Pagination.Total.Should().Be(0);
            response.Pagination.Pages.Should().Be(0);
        }

        [Fact]
        public async Task GetPlayerStats_ShouldPassFilterToService()
        {
            // Arrange
            var filter = new PlayerStatsFilterDto
            {
                MatchId = 1,
                PlayerId = 10,
                TeamId = 5,
                Position = "GK",
                Sort = "form",
                Order = "asc",
                Page = 2,
                Limit = 25
            };
            var stats = new List<PlayerStatsDto>();
            var total = 0;

            _mockPlayerStatsService.Setup(x => x.GetStatsAsync(filter))
                .ReturnsAsync((stats, total));

            // Act
            await _controller.GetPlayerStats(filter);

            // Assert
            _mockPlayerStatsService.Verify(x => x.GetStatsAsync(It.Is<PlayerStatsFilterDto>(
                f => f.MatchId == 1 &&
                     f.PlayerId == 10 &&
                     f.TeamId == 5 &&
                     f.Position == "GK" &&
                     f.Sort == "form" &&
                     f.Order == "asc" &&
                     f.Page == 2 &&
                     f.Limit == 25)), Times.Once);
        }

        #endregion

        #region ImportStats Tests

        [Fact]
        public async Task ImportStats_WhenImportSucceeds_ShouldReturnCreatedWithResponse()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var request = new PlayerStatsImportRequestDto { File = mockFile.Object };
            var importResponse = new PlayerStatsImportResponseDto
            {
                Success = true,
                ImportedCount = 50,
                SkippedCount = 2,
                Errors = new List<string> { "Some validation error" }
            };

            _mockPlayerStatsService.Setup(x => x.ImportFromCsvAsync(request))
                .ReturnsAsync(importResponse);

            // Act
            var result = await _controller.ImportStats(request);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
            createdResult.Value.Should().BeEquivalentTo(importResponse);
        }

        [Fact]
        public async Task ImportStats_WhenImportFails_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var request = new PlayerStatsImportRequestDto { File = mockFile.Object };
            var importResponse = new PlayerStatsImportResponseDto
            {
                Success = false,
                ImportedCount = 0,
                SkippedCount = 0,
                Errors = new List<string> { "File format error", "Invalid data" }
            };

            _mockPlayerStatsService.Setup(x => x.ImportFromCsvAsync(request))
                .ReturnsAsync(importResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _controller.ImportStats(request));
            exception.Message.Should().Contain("Import failed: File format error, Invalid data");
        }

        [Fact]
        public async Task ImportStats_WhenImportFailsWithEmptyErrors_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var request = new PlayerStatsImportRequestDto { File = mockFile.Object };
            var importResponse = new PlayerStatsImportResponseDto
            {
                Success = false,
                ImportedCount = 0,
                SkippedCount = 0,
                Errors = new List<string>()
            };

            _mockPlayerStatsService.Setup(x => x.ImportFromCsvAsync(request))
                .ReturnsAsync(importResponse);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _controller.ImportStats(request));
            exception.Message.Should().Contain("Import failed:");
        }

        #endregion
    }
}
