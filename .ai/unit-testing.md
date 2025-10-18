# Wytyczne Testowania Jednostkowego dla Fantasy Coach AI

## 📋 Przegląd

Ten dokument opisuje kompleksowe strategie testowania jednostkowego dla aplikacji Fantasy Coach AI, obejmujące wszystkie warstwy implementacji Clean Architecture.

## 🏗️ Architektura Testów

### Struktura Testów według Warstw

```
src/
├── FantasyCoachAI.Domain.Tests/           # Testy warstwy domenowej
│   ├── Entities/                          # Testy logiki biznesowej encji
│   ├── Enums/                            # Testy walidacji enumów
│   └── Exceptions/                        # Testy niestandardowych wyjątków
├── FantasyCoachAI.Application.Tests/     # Testy warstwy aplikacji
│   ├── Services/                         # Testy logiki biznesowej serwisów
│   ├── Validators/                       # Testy FluentValidation
│   ├── DTOs/                            # Testy obiektów transferu danych
│   ├── Performance/                     # Testy wydajności
│   ├── Async/                           # Testy operacji asynchronicznych
│   └── Mocking/                         # Wzorce testowania z mockami
├── FantasyCoachAI.Infrastructure.Tests/ # Testy warstwy infrastruktury
│   ├── Repositories/                     # Testy dostępu do danych
│   ├── Mappers/                         # Testy mapowania obiektów
│   ├── Configuration/                   # Testy konfiguracji
│   └── Integration/                     # Testy integracyjne z bazą danych
└── FantasyCoachAI.Web.Tests/            # Testy warstwy prezentacji
    ├── Controllers/                      # Testy kontrolerów API
    ├── Middleware/                      # Testy middleware
    ├── Filters/                         # Testy filtrów akcji
    ├── Security/                        # Testy bezpieczeństwa
    ├── Integration/                     # Testy end-to-end
    └── EndToEnd/                        # Testy pełnego systemu
```

## 🎯 Zasady Testowania

### 1. **Kategorie Testów**

#### **Testy Jednostkowe**
- Testują pojedyncze komponenty w izolacji
- Używają mocków dla zależności
- Szybkie wykonanie (< 100ms na test)
- Wysokie pokrycie kodu (> 90%)

#### **Testy Integracyjne**
- Testują interakcje między komponentami
- Używają rzeczywistych zależności gdzie to możliwe
- Testują przepływ danych między warstwami
- Umiarkowany czas wykonania (< 1s na test)

#### **Testy Wydajności**
- Testują z dużymi zbiorami danych (10,000+ rekordów)
- Mierzą czas wykonania i użycie pamięci
- Testują operacje współbieżne
- Scenariusze testów obciążeniowych

#### **Testy Bezpieczeństwa**
- Ochrona przed iniekcją SQL
- Zapobieganie atakom XSS
- Walidacja danych wejściowych
- Sprawdzanie autoryzacji

### 2. **Konwencja Nazewnictwa Testów**

```csharp
// Wzorzec: NazwaMetody_Scenariusz_OczekiwanyWynik
[Fact]
public void GetByIdAsync_WhenValidId_ShouldReturnTeam()
{
    // Arrange, Act, Assert
}

[Theory]
[InlineData(1, true)]
[InlineData(0, false)]
public void IsValidId_WhenCalled_ShouldReturnCorrectValue(int id, bool expected)
{
    // Implementacja testu
}
```

### 3. **Struktura Testu (Wzorzec AAA)**

```csharp
[Fact]
public void ExampleTest_WhenCondition_ShouldReturnExpectedResult()
{
    // Arrange - Przygotowanie danych testowych i mocków
    var mockRepository = new Mock<ITeamRepository>();
    var expectedTeam = new Team { Id = 1, Name = "Test Team" };
    mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(expectedTeam);
    
    // Act - Wykonanie testowanej metody
    var result = await service.GetByIdAsync(1);
    
    // Assert - Weryfikacja wyników
    result.Should().NotBeNull();
    result.Id.Should().Be(1);
    result.Name.Should().Be("Test Team");
}
```

## 🔧 Testowanie Warstwy Domenowej

### **Testy Encji**

#### **Encja Gameweek**
```csharp
public class GameweekTests
{
    [Theory]
    [InlineData("2025-01-01", "2025-01-10", "2024-12-31", GameweekStatus.Upcoming)]
    [InlineData("2025-01-01", "2025-01-10", "2025-01-01", GameweekStatus.Current)]
    [InlineData("2025-01-01", "2025-01-10", "2025-01-11", GameweekStatus.Completed)]
    public void GetStatus_WhenCalled_ShouldReturnCorrectStatus(
        string startDateStr, string endDateStr, string currentDateStr, GameweekStatus expectedStatus)
    {
        // Arrange
        var startDate = DateTime.Parse(startDateStr);
        var endDate = DateTime.Parse(endDateStr);
        var currentDate = DateTime.Parse(currentDateStr);
        var gameweek = new Gameweek
        {
            StartDate = startDate,
            EndDate = endDate
        };

        // Act
        var result = gameweek.GetStatus(currentDate);

        // Assert
        result.Should().Be(expectedStatus);
    }
    
    [Fact]
    public void IsValidDateRange_WhenStartDateBeforeEndDate_ShouldReturnTrue()
    {
        // Arrange
        var startDate = DateTime.Parse("2025-01-01");
        var endDate = DateTime.Parse("2025-01-10");
        var gameweek = new Gameweek
        {
            StartDate = startDate,
            EndDate = endDate
        };

        // Act
        var result = gameweek.IsValidDateRange();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidDateRange_WhenStartDateAfterEndDate_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateTime.Parse("2025-01-10");
        var endDate = DateTime.Parse("2025-01-01");
        var gameweek = new Gameweek
        {
            StartDate = startDate,
            EndDate = endDate
        };

        // Act
        var result = gameweek.IsValidDateRange();

        // Assert
        result.Should().BeFalse();
    }
}
```

#### **Encja Match**
```csharp
public class MatchTests
{
    [Theory]
    [InlineData(MatchStatus.Scheduled, false)]
    [InlineData(MatchStatus.Live, false)]
    [InlineData(MatchStatus.Finished, true)]
    public void IsFinished_WhenCalled_ShouldReturnCorrectValue(MatchStatus status, bool expected)
    {
        // Arrange
        var match = new Match
        {
            Status = status
        };

        // Act
        var result = match.IsFinished();

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void IsValidTeams_WhenDifferentTeams_ShouldReturnTrue()
    {
        // Arrange
        var homeTeam = new Team { Id = 1, Name = "Team A" };
        var awayTeam = new Team { Id = 2, Name = "Team B" };
        var match = new Match
        {
            HomeTeam = homeTeam,
            AwayTeam = awayTeam
        };

        // Act
        var result = match.IsValidTeams();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidTeams_WhenSameTeam_ShouldReturnFalse()
    {
        // Arrange
        var team = new Team { Id = 1, Name = "Team A" };
        var match = new Match
        {
            HomeTeam = team,
            AwayTeam = team
        };

        // Act
        var result = match.IsValidTeams();

        // Assert
        result.Should().BeFalse();
    }
}
```

#### **Encja Team**
```csharp
public class TeamTests
{
    [Theory]
    [InlineData("https://example.com/crest.png", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void HasCrest_WhenCalled_ShouldReturnCorrectValue(string crestUrl, bool expected)
    {
        // Arrange
        var team = new Team
        {
            CrestUrl = crestUrl
        };

        // Act
        var result = team.HasCrest();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsActive_WhenTeamIsActive_ShouldReturnTrue()
    {
        // Arrange
        var team = new Team
        {
            IsActive = true
        };

        // Act
        var result = team.IsActive;

        // Assert
        result.Should().BeTrue();
    }
}
```

## 🏢 Testowanie Warstwy Aplikacji

### **Testy Serwisów**

#### **Testy TeamService**
```csharp
public class TeamServiceTests
{
    private readonly Mock<ITeamRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TeamService _service;

    public TeamServiceTests()
    {
        _mockRepository = new Mock<ITeamRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new TeamService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetTeamsAsync_WhenValidFilter_ShouldReturnTeams()
    {
        // Arrange
        var filter = new TeamFilterDto { IsActive = true };
        var teams = new List<Team> { TestDataBuilder.CreateValidTeam() };
        var teamDtos = new List<TeamDto> { new TeamDto { Id = 1, Name = "Test Team" } };

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<TeamFilter>()))
            .ReturnsAsync(teams);
        _mockMapper.Setup(x => x.Map<List<TeamDto>>(teams))
            .Returns(teamDtos);

        // Act
        var result = await _service.GetTeamsAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<TeamFilter>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WhenValidCommand_ShouldCreateTeam()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "New Team",
            ShortCode = "NEW"
        };
        var team = TestDataBuilder.CreateValidTeam();
        var teamDto = new TeamDto { Id = 1, Name = "New Team" };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Team>()))
            .ReturnsAsync(team);
        _mockMapper.Setup(x => x.Map<Team>(command))
            .Returns(team);
        _mockMapper.Setup(x => x.Map<TeamDto>(team))
            .Returns(teamDto);

        // Act
        var result = await _service.CreateAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Team");
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Team>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateAsync_WhenTeamAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Existing Team",
            ShortCode = "EXT"
        };

        _mockRepository.Setup(x => x.ExistsByNameAsync(command.Name))
            .ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(command))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Team with this name already exists");
    }
}
```

#### **Testy MatchService**
```csharp
public class MatchServiceTests
{
    private readonly Mock<IMatchRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MatchService _service;

    public MatchServiceTests()
    {
        _mockRepository = new Mock<IMatchRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new MatchService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenSameTeam_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateMatchCommand
        {
            HomeTeamId = 1,
            AwayTeamId = 1,
            MatchDate = DateTime.Now.AddDays(1)
        };

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(command))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Team cannot play against itself");
    }
    
    [Fact]
    public async Task CreateAsync_WhenMatchDateInPast_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateMatchCommand
        {
            HomeTeamId = 1,
            AwayTeamId = 2,
            MatchDate = DateTime.Now.AddDays(-1)
        };

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(command))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Match date cannot be in the past");
    }
}
```

### **Testy Walidatorów**

#### **Testy CreateTeamCommandValidator**
```csharp
public class CreateTeamCommandValidatorTests
{
    private readonly CreateTeamCommandValidator _validator;

    public CreateTeamCommandValidatorTests()
    {
        _validator = new CreateTeamCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenNameIsNullOrWhitespace_ShouldHaveValidationError(string name)
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = name,
            ShortCode = "TST"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }
    
    [Theory]
    [InlineData("A")]      // Too short
    [InlineData("ABCD")]   // Too long
    [InlineData("a1")]     // Contains number
    [InlineData("AB-")]    // Contains special character
    public void Validate_WhenShortCodeIsInvalid_ShouldHaveValidationError(string shortCode)
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Test Team",
            ShortCode = shortCode
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ShortCode");
    }

    [Fact]
    public void Validate_WhenValidCommand_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Test Team",
            ShortCode = "TST"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
```

## 🗄️ Testowanie Warstwy Infrastruktury

### **Testy Repozytoriów**

#### **Testy TeamRepository**
```csharp
public class TeamRepositoryTests : IDisposable
{
    private readonly SupabaseClient _client;
    private readonly TeamRepository _repository;

    public TeamRepositoryTests()
    {
        // Setup test database connection
        _client = new SupabaseClient(TestConfiguration.SupabaseUrl, TestConfiguration.SupabaseKey);
        _repository = new TeamRepository(_client);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ShouldReturnAllTeams()
    {
        // Arrange
        var expectedTeams = new List<Team>
        {
            TestDataBuilder.CreateValidTeam(1),
            TestDataBuilder.CreateValidTeam(2)
        };

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterOrEqualTo(2);
    }
    
    [Fact]
    public async Task CreateAsync_WhenValidTeam_ShouldCreateTeam()
    {
        // Arrange
        var team = TestDataBuilder.CreateValidTeam();

        // Act
        var result = await _repository.CreateAsync(team);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be(team.Name);
    }
    
    [Fact]
    public async Task GetAllAsync_WhenSupabaseThrowsException_ShouldPropagateException()
    {
        // Arrange
        var invalidClient = new SupabaseClient("invalid-url", "invalid-key");
        var repository = new TeamRepository(invalidClient);

        // Act & Assert
        await repository.Invoking(r => r.GetAllAsync())
            .Should().ThrowAsync<Exception>();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
```

### **Testy Mapperów**

#### **Testy TeamMapper**
```csharp
public class TeamMapperTests
{
    private readonly TeamMapper _mapper;

    public TeamMapperTests()
    {
        _mapper = new TeamMapper();
    }

    [Fact]
    public void ToDto_WhenValidTeam_ShouldMapCorrectly()
    {
        // Arrange
        var team = TestDataBuilder.CreateValidTeam();

        // Act
        var result = _mapper.ToDto(team);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(team.Id);
        result.Name.Should().Be(team.Name);
        result.ShortCode.Should().Be(team.ShortCode);
        result.IsActive.Should().Be(team.IsActive);
    }
    
    [Fact]
    public void ToEntity_WhenValidCreateTeamCommand_ShouldMapCorrectly()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "Test Team",
            ShortCode = "TST"
        };

        // Act
        var result = _mapper.ToEntity(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.ShortCode.Should().Be(command.ShortCode);
        result.IsActive.Should().BeTrue();
    }
    
    [Fact]
    public void RoundTrip_WhenMappingTeamToDtoAndBack_ShouldPreserveData()
    {
        // Arrange
        var originalTeam = TestDataBuilder.CreateValidTeam();

        // Act
        var dto = _mapper.ToDto(originalTeam);
        var mappedBack = _mapper.ToEntity(dto);

        // Assert
        mappedBack.Name.Should().Be(originalTeam.Name);
        mappedBack.ShortCode.Should().Be(originalTeam.ShortCode);
        mappedBack.IsActive.Should().Be(originalTeam.IsActive);
    }
}
```

## 🌐 Testowanie Warstwy Web

### **Testy Kontrolerów**

#### **Testy TeamsController**
```csharp
public class TeamsControllerTests
{
    private readonly Mock<ITeamService> _mockService;
    private readonly TeamsController _controller;

    public TeamsControllerTests()
    {
        _mockService = new Mock<ITeamService>();
        _controller = new TeamsController(_mockService.Object);
    }

    [Fact]
    public async Task GetTeams_WhenCalled_ShouldReturnOkResultWithTeams()
    {
        // Arrange
        var teams = new List<TeamDto>
        {
            new TeamDto { Id = 1, Name = "Team A" },
            new TeamDto { Id = 2, Name = "Team B" }
        };
        var filter = new TeamFilterDto();

        _mockService.Setup(x => x.GetTeamsAsync(filter))
            .ReturnsAsync(teams);

        // Act
        var result = await _controller.GetTeams(filter);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(teams);
    }
    
    [Fact]
    public async Task GetTeam_WhenTeamNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var teamId = 999;
        _mockService.Setup(x => x.GetByIdAsync(teamId))
            .ThrowsAsync(new NotFoundException("Team not found"));

        // Act
        var result = await _controller.GetTeam(teamId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
    
    [Fact]
    public async Task CreateTeam_WhenValidCommand_ShouldReturnCreatedResult()
    {
        // Arrange
        var command = new CreateTeamCommand
        {
            Name = "New Team",
            ShortCode = "NEW"
        };
        var teamDto = new TeamDto { Id = 1, Name = "New Team" };

        _mockService.Setup(x => x.CreateAsync(command))
            .ReturnsAsync(teamDto);

        // Act
        var result = await _controller.CreateTeam(command);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult.Value.Should().BeEquivalentTo(teamDto);
    }
}
```

### **Testy Middleware**

#### **Testy ApiExceptionMiddleware**
```csharp
public class ApiExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_ShouldReturn404WithMessage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var middleware = new ApiExceptionMiddleware(next: (innerHttpContext) =>
        {
            throw new NotFoundException("Resource not found");
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
        context.Response.ContentType.Should().Be("application/json");
    }
    
    [Fact]
    public async Task InvokeAsync_WhenArgumentException_ShouldReturn400WithMessage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        
        var middleware = new ApiExceptionMiddleware(next: (innerHttpContext) =>
        {
            throw new ArgumentException("Invalid argument");
        });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().Be("application/json");
    }
}
```

## 🔒 Testowanie Bezpieczeństwa

### **Testy Bezpieczeństwa**
```csharp
public class SecurityTests
{
    private readonly TeamsController _controller;
    private readonly Mock<ITeamService> _mockService;

    public SecurityTests()
    {
        _mockService = new Mock<ITeamService>();
        _controller = new TeamsController(_mockService.Object);
    }

    [Theory]
    [InlineData("'; DROP TABLE teams; --")]
    [InlineData("' OR '1'='1")]
    [InlineData("'; DELETE FROM teams; --")]
    public async Task GetTeams_WhenShortCodeContainsSqlInjection_ShouldNotExecuteSql(string maliciousInput)
    {
        // Arrange
        var filter = new TeamFilterDto { ShortCode = maliciousInput };
        _mockService.Setup(x => x.GetTeamsAsync(filter))
            .ReturnsAsync(new List<TeamDto>());

        // Act
        var result = await _controller.GetTeams(filter);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockService.Verify(x => x.GetTeamsAsync(filter), Times.Once);
    }
    
    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("javascript:alert('xss')")]
    [InlineData("<img src=x onerror=alert('xss')>")]
    public async Task GetTeams_WhenShortCodeContainsXss_ShouldNotExecuteScript(string maliciousInput)
    {
        // Arrange
        var filter = new TeamFilterDto { ShortCode = maliciousInput };
        _mockService.Setup(x => x.GetTeamsAsync(filter))
            .ReturnsAsync(new List<TeamDto>());

        // Act
        var result = await _controller.GetTeams(filter);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        // Verify that the malicious script is not executed
        _mockService.Verify(x => x.GetTeamsAsync(filter), Times.Once);
    }
}
```

## ⚡ Testowanie Wydajności

### **Testy Wydajności**
```csharp
public class PerformanceTests
{
    [Fact]
    public async Task GetTeamsAsync_WithLargeDataset_ShouldNotExceedMemoryLimit()
    {
        // Arrange
        var largeTeamList = TestDataBuilder.CreateLargeTeamList(10000);
        var mockRepository = new Mock<ITeamRepository>();
        var mockMapper = new Mock<IMapper>();
        
        mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(largeTeamList);
        mockMapper.Setup(x => x.Map<List<TeamDto>>(It.IsAny<List<Team>>()))
            .Returns(largeTeamList.Select(t => new TeamDto { Id = t.Id, Name = t.Name }).ToList());

        var service = new TeamService(mockRepository.Object, mockMapper.Object);

        // Act
        var startMemory = GC.GetTotalMemory(true);
        var result = await service.GetTeamsAsync(new TeamFilterDto());
        var endMemory = GC.GetTotalMemory(false);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(10000);
        (endMemory - startMemory).Should().BeLessThan(50 * 1024 * 1024); // Less than 50MB
    }
    
    [Fact]
    public async Task MultipleConcurrentOperations_ShouldCompleteWithinTimeLimit()
    {
        // Arrange
        var tasks = new List<Task>();
        var startTime = DateTime.UtcNow;

        // Act
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                // Simulate some async operation
                await Task.Delay(10);
            }));
        }

        await Task.WhenAll(tasks);
        var endTime = DateTime.UtcNow;

        // Assert
        (endTime - startTime).TotalSeconds.Should().BeLessThan(5);
    }
}
```

## 🧪 Narzędzia Testowe

### **Konstruktory Danych Testowych**
```csharp
public static class TestDataBuilder
{
    public static Team CreateValidTeam(int id = 1) => new Team
    {
        Id = id,
        Name = "Test Team",
        ShortCode = "TST",
        IsActive = true,
        CrestUrl = "https://example.com/crest.png"
    };
    
    public static List<Team> CreateLargeTeamList(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new Team
            {
                Id = i,
                Name = $"Team {i}",
                ShortCode = $"T{i:D3}",
                IsActive = i % 2 == 0,
                CrestUrl = $"https://example.com/crest_{i}.png"
            })
            .ToList();
    }

    public static Gameweek CreateValidGameweek(int id = 1) => new Gameweek
    {
        Id = id,
        Name = $"Gameweek {id}",
        StartDate = DateTime.Now.AddDays(id * 7),
        EndDate = DateTime.Now.AddDays((id * 7) + 6),
        IsActive = true
    };

    public static Match CreateValidMatch(int id = 1) => new Match
    {
        Id = id,
        HomeTeam = CreateValidTeam(1),
        AwayTeam = CreateValidTeam(2),
        MatchDate = DateTime.Now.AddDays(1),
        Status = MatchStatus.Scheduled
    };
}
```

### **Pomocniki Konfiguracji Mocków**
```csharp
public static class MockSetupHelper
{
    public static void SetupTeamRepository(Mock<ITeamRepository> mock, List<Team> teams)
    {
        mock.Setup(x => x.GetAllAsync()).ReturnsAsync(teams);
        mock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => teams.FirstOrDefault(t => t.Id == id));
        mock.Setup(x => x.ExistsByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((string name) => teams.Any(t => t.Name == name));
    }

    public static void SetupTeamService(Mock<ITeamService> mock, List<TeamDto> teams)
    {
        mock.Setup(x => x.GetTeamsAsync(It.IsAny<TeamFilterDto>()))
            .ReturnsAsync(teams);
        mock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => teams.FirstOrDefault(t => t.Id == id));
    }
}
```

## 📊 Wymagania Pokrycia Testów

### **Minimalne Cele Pokrycia**
- **Warstwa Domenowa**: 95%+ pokrycia
- **Warstwa Aplikacji**: 90%+ pokrycia
- **Warstwa Infrastruktury**: 85%+ pokrycia
- **Warstwa Web**: 80%+ pokrycia

### **Pokrycie Ścieżek Krytycznych**
- Wszystkie reguły biznesowe muszą być przetestowane
- Wszystkie scenariusze błędów muszą być pokryte
- Wszystkie przypadki brzegowe muszą być przetestowane
- Wszystkie luki bezpieczeństwa muszą być przetestowane

## 🚀 Uruchamianie Testów

### **Wiersz Poleceń**
```bash
# Uruchom wszystkie testy
dotnet test

# Uruchom określoną kategorię testów
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
dotnet test --filter Category=Performance
dotnet test --filter Category=Security

# Uruchom z pokryciem kodu
dotnet test --collect:"XPlat Code Coverage"

# Uruchom określoną klasę testów
dotnet test --filter ClassName=TeamServiceTests

# Uruchom testy z raportowaniem
dotnet test --logger "trx;LogFileName=test-results.trx"
```

### **Kategorie Testów**
```csharp
[Trait("Category", "Unit")]
[Trait("Category", "Integration")]
[Trait("Category", "Performance")]
[Trait("Category", "Security")]
```

## 📈 Ciągła Integracja

### **Testy w Pipeline CI**
1. **Testy Jednostkowe** - Uruchamiane przy każdym commicie
2. **Testy Integracyjne** - Uruchamiane przy pull requestach
3. **Testy Wydajności** - Uruchamiane codziennie
4. **Testy Bezpieczeństwa** - Uruchamiane przy wdrożeniu

### **Raporty Testów**
- Raporty pokrycia kodu
- Benchmarki wydajności
- Wyniki skanowania bezpieczeństwa
- Raporty wykonania testów

## 🔍 Debugowanie Testów

### **Typowe Problemy**
1. **Async/Await** - Zawsze używaj `await` w testach asynchronicznych
2. **Konfiguracja Mocków** - Weryfikuj konfiguracje mocków
3. **Dane Testowe** - Używaj spójnych danych testowych
4. **Asercje** - Używaj opisowych komunikatów asercji

### **Najlepsze Praktyki**
1. **Jedna Asercja na Test** - Zachowaj skupienie testów
2. **Opisowe Nazwy** - Używaj jasnych nazw testów
3. **Niezależne Testy** - Testy nie powinny zależeć od siebie
4. **Szybkie Testy** - Zachowaj szybkość testów jednostkowych
5. **Niezawodne Testy** - Testy powinny być deterministyczne

## 📚 Dodatkowe Zasoby

- [Dokumentacja xUnit](https://xunit.net/)
- [Dokumentacja FluentAssertions](https://fluentassertions.com/)
- [Dokumentacja Moq](https://github.com/moq/moq4)
- [Testowanie Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Wzorce Testowania .NET](https://docs.microsoft.com/en-us/dotnet/core/testing/)

---

*Ten dokument służy jako kompleksowy przewodnik testowania jednostkowego aplikacji Fantasy Coach AI, zapewniając wysoką jakość, łatwość utrzymania i niezawodność kodu.*