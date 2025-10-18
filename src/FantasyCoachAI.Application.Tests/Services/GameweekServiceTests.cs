using FantasyCoachAI.Application.DTOs;
using FantasyCoachAI.Application.Services;
using FantasyCoachAI.Domain.Entities;
using FantasyCoachAI.Domain.Enums;
using FantasyCoachAI.Domain.Interfaces;

namespace FantasyCoachAI.Application.Tests.Services
{
    [Trait("Category", "Unit")]
    public class GameweekServiceTests
    {
        private readonly Mock<IGameweekRepository> _mockGameweekRepository;
        private readonly Mock<IMatchRepository> _mockMatchRepository;
        private readonly GameweekService _gameweekService;

        public GameweekServiceTests()
        {
            _mockGameweekRepository = new Mock<IGameweekRepository>();
            _mockMatchRepository = new Mock<IMatchRepository>();
            _gameweekService = new GameweekService(_mockGameweekRepository.Object, _mockMatchRepository.Object);
        }

        #region GetGameweeksAsync Tests

        [Fact]
        public async Task GetGameweeksAsync_WhenFilterProvided_ShouldReturnFilteredGameweeks()
        {
            // Arrange
            var filter = new GameweekFilterDto
            {
                Status = GameweekStatus.Current,
                Sort = "start_date",
                Order = "desc"
            };
            var expectedGameweeks = new List<Gameweek>
            {
                new() { Id = 3, Number = 15, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(1) }
            };

            _mockGameweekRepository.Setup(x => x.GetFilteredAsync(filter.Status, filter.Sort, false))
                .ReturnsAsync(expectedGameweeks);

            // Act
            var result = await _gameweekService.GetGameweeksAsync(filter);

            // Assert
            result.Should().HaveCount(1);
            result[0].Number.Should().Be(15);
            result[0].Status.Should().Be(GameweekStatus.Current);
            _mockGameweekRepository.Verify(x => x.GetFilteredAsync(filter.Status, filter.Sort, false), Times.Once);
        }

        #endregion

        #region GetGameweekByIdAsync Tests

        [Fact]
        public async Task GetGameweekByIdAsync_WhenIdIsValid_ShouldReturnGameweekDto()
        {
            // Arrange
            var gameweekId = 7;
            var expectedGameweek = new Gameweek
            {
                Id = gameweekId,
                Number = 12,
                StartDate = new DateTime(2025, 11, 1),
                EndDate = new DateTime(2025, 11, 3)
            };

            _mockGameweekRepository.Setup(x => x.GetByIdAsync(gameweekId))
                .ReturnsAsync(expectedGameweek);

            // Act
            var result = await _gameweekService.GetGameweekByIdAsync(gameweekId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(gameweekId);
            result.Number.Should().Be(12);
            _mockGameweekRepository.Verify(x => x.GetByIdAsync(gameweekId), Times.Once);
        }

        [Fact]
        public async Task GetGameweekByIdAsync_WhenGameweekNotFound_ShouldReturnNull()
        {
            // Arrange
            var gameweekId = 999;

            _mockGameweekRepository.Setup(x => x.GetByIdAsync(gameweekId))
                .ReturnsAsync((Gameweek?)null);

            // Act
            var result = await _gameweekService.GetGameweekByIdAsync(gameweekId);

            // Assert
            result.Should().BeNull();
            _mockGameweekRepository.Verify(x => x.GetByIdAsync(gameweekId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetGameweekByIdAsync_WhenIdIsInvalid_ShouldThrowArgumentException(int invalidId)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameweekService.GetGameweekByIdAsync(invalidId));
            exception.Message.Should().Contain("Id must be greater than zero");
            exception.ParamName.Should().Be("id");
        }

        #endregion

        #region CreateGameweekAsync Tests

        [Fact]
        public async Task CreateGameweekAsync_WhenCommandIsValid_ShouldCreateGameweek()
        {
            // Arrange
            var command = new CreateGameweekCommand
            {
                Number = 20,
                StartDate = new DateTime(2026, 1, 15),
                EndDate = new DateTime(2026, 1, 17)
            };

            var createdGameweek = new Gameweek
            {
                Id = 10,
                Number = command.Number,
                StartDate = command.StartDate,
                EndDate = command.EndDate
            };

            _mockGameweekRepository.Setup(x => x.GetByNumberAsync(command.Number))
                .ReturnsAsync((Gameweek?)null);
            _mockGameweekRepository.Setup(x => x.CreateAsync(It.IsAny<Gameweek>()))
                .ReturnsAsync(createdGameweek);

            // Act
            var result = await _gameweekService.CreateGameweekAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(10);
            result.Number.Should().Be(20);
            result.StartDate.Should().Be(command.StartDate);
            result.EndDate.Should().Be(command.EndDate);
            _mockGameweekRepository.Verify(x => x.GetByNumberAsync(command.Number), Times.Once);
            _mockGameweekRepository.Verify(x => x.CreateAsync(It.Is<Gameweek>(g =>
                g.Number == command.Number &&
                g.StartDate == command.StartDate &&
                g.EndDate == command.EndDate)), Times.Once);
        }

        [Fact]
        public async Task CreateGameweekAsync_WhenCommandIsNull_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _gameweekService.CreateGameweekAsync(null!));
            exception.ParamName.Should().Be("command");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public async Task CreateGameweekAsync_WhenNumberIsInvalid_ShouldThrowArgumentException(int invalidNumber)
        {
            // Arrange
            var command = new CreateGameweekCommand
            {
                Number = invalidNumber,
                StartDate = new DateTime(2026, 1, 15),
                EndDate = new DateTime(2026, 1, 17)
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameweekService.CreateGameweekAsync(command));
            exception.Message.Should().Contain("Gameweek number must be greater than zero");
        }

        [Fact]
        public async Task CreateGameweekAsync_WhenStartDateIsAfterEndDate_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateGameweekCommand
            {
                Number = 20,
                StartDate = new DateTime(2026, 1, 17),
                EndDate = new DateTime(2026, 1, 15) // End before start
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameweekService.CreateGameweekAsync(command));
            exception.Message.Should().Contain("Start date must be before end date");
        }

        [Fact]
        public async Task CreateGameweekAsync_WhenStartDateEqualsEndDate_ShouldThrowArgumentException()
        {
            // Arrange
            var date = new DateTime(2026, 1, 15);
            var command = new CreateGameweekCommand
            {
                Number = 20,
                StartDate = date,
                EndDate = date // Same date
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameweekService.CreateGameweekAsync(command));
            exception.Message.Should().Contain("Start date must be before end date");
        }

        [Fact]
        public async Task CreateGameweekAsync_WhenGameweekNumberAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var command = new CreateGameweekCommand
            {
                Number = 15,
                StartDate = new DateTime(2026, 1, 15),
                EndDate = new DateTime(2026, 1, 17)
            };

            var existingGameweek = new Gameweek
            {
                Id = 1,
                Number = 15,
                StartDate = new DateTime(2025, 10, 1),
                EndDate = new DateTime(2025, 10, 3)
            };

            _mockGameweekRepository.Setup(x => x.GetByNumberAsync(command.Number))
                .ReturnsAsync(existingGameweek);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _gameweekService.CreateGameweekAsync(command));
            exception.Message.Should().Contain("Gameweek with number 15 already exists");
        }

        [Fact]
        public async Task CreateGameweekAsync_WhenStartDateIsTooFarInPast_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateGameweekCommand
            {
                Number = 20,
                StartDate = DateTime.UtcNow.AddYears(-2), // Too far in past
                EndDate = DateTime.UtcNow.AddYears(-2).AddDays(2)
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameweekService.CreateGameweekAsync(command));
            exception.Message.Should().Contain("Start date cannot be more than 1 year in the past");
        }

        [Fact]
        public async Task CreateGameweekAsync_WhenEndDateIsTooFarInFuture_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateGameweekCommand
            {
                Number = 20,
                StartDate = DateTime.UtcNow.AddYears(3), // Too far in future
                EndDate = DateTime.UtcNow.AddYears(3).AddDays(2)
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _gameweekService.CreateGameweekAsync(command));
            exception.Message.Should().Contain("End date cannot be more than 2 years in the future");
        }

        #endregion
    }
}
