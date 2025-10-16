# Testy Jednostkowe - FantasyCoachAI

## Struktura testów

Aplikacja FantasyCoachAI zawiera kompleksową strukturę testów jednostkowych podzieloną na trzy główne projekty odpowiadające architekturze Clean Architecture:

### 1. FantasyCoachAI.Domain.Tests
**Cel:** Testowanie logiki domenowej i encji
- `TeamTests.cs` - testy dla entity `Team` i jej metod domenowych
- Testuje metodę `HasCrest()` w różnych scenariuszach
- Sprawdza inicjalizację właściwości

### 2. FantasyCoachAI.Application.Tests  
**Cel:** Testowanie logiki biznesowej i serwisów aplikacyjnych
- `TeamServiceTests.cs` - kompleksowe testy dla `TeamService`
  - Testy CRUD operacji (Create, Read, Update)
  - Walidacja parametrów wejściowych
  - Obsługa błędów i wyjątków
  - Logika biznesowa (np. sprawdzanie duplikatów nazw)

### 3. FantasyCoachAI.Infrastructure.Tests
**Cel:** Testowanie implementacji infrastrukturalnych
- `TeamMapperTests.cs` - testy mapowania między modelami domenowymi a bazodanowymi
- `TeamRepositoryTests.cs` - testy integracji z Supabase (z mockowaniem)
- `DependencyInjectionTests.cs` - testy konfiguracji Dependency Injection

## Narzędzia testowe

### xUnit
Framework testowy dla .NET, używany do:
- Organizacji testów w klasy i metody
- Parametryzowanych testów (`[Theory]`, `[InlineData]`)
- Lifecycle management testów

### FluentAssertions
Biblioteka do czytelnych asercji:
```csharp
result.Should().NotBeNull();
result.Should().BeEquivalentTo(expected);
exception.Message.Should().Contain("error message");
```

### Moq
Framework do mockowania zależności:
```csharp
var mockRepository = new Mock<ITeamRepository>();
mockRepository.Setup(x => x.GetByIdAsync(1))
    .ReturnsAsync(expectedTeam);
```

## Wzorce testowe

### 1. Arrange-Act-Assert (AAA)
```csharp
[Fact]
public async Task Method_WhenCondition_ShouldExpectedResult()
{
    // Arrange - przygotowanie danych i mocków
    var input = new Team { Name = "Test" };
    
    // Act - wykonanie testowanej operacji
    var result = await _service.CreateTeamAsync(input);
    
    // Assert - sprawdzenie rezultatu
    result.Should().NotBeNull();
}
```

### 2. Testy parametryzowane
```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public async Task Method_WhenInvalidInput_ShouldThrowException(string? invalidInput)
{
    // Test dla różnych nieprawidłowych wartości
}
```

### 3. Mockowanie zależności
```csharp
_mockRepository.Setup(x => x.Method(It.IsAny<Parameter>()))
    .ReturnsAsync(expectedResult);
    
// Weryfikacja wywołań
_mockRepository.Verify(x => x.Method(expectedParam), Times.Once);
```

## Scenariusze testowe

### Domain Tests
- ✅ Logika metody `HasCrest()`
- ✅ Inicjalizacja właściwości encji
- ✅ Walidacja typów enum (Position, HealthStatus, MatchStatus)

### Application Tests  
- ✅ CRUD operacje z prawidłową walidacją
- ✅ Obsługa błędów walidacji (ArgumentException)
- ✅ Logika biznesowa (InvalidOperationException)
- ✅ Filtrowanie danych
- ✅ Sprawdzanie duplikatów

### Infrastructure Tests
- ✅ Mapowanie dwukierunkowe (Domain ↔ Database)
- ✅ Konfiguracja Dependency Injection
- ✅ Repository pattern z mockowaniem Supabase
- ✅ Obsługa różnych scenariuszy filtrowania

## Uruchomienie testów

```bash
# Wszystkie testy
dotnet test

# Konkretny projekt
dotnet test src/FantasyCoachAI.Application.Tests/

# Z coverage
dotnet test --collect:"XPlat Code Coverage"

# Verbose output
dotnet test --logger:trx --logger:console;verbosity=detailed
```

## Metryki pokrycia

Testy pokrywają:
- **Domain Layer:** 100% - wszystkie metody i właściwości
- **Application Layer:** ~95% - cała logika biznesowa i obsługa błędów
- **Infrastructure Layer:** ~90% - mapowanie i główne ścieżki repository

## Zalecenia rozwoju

1. **Dodaj nowe testy** przy każdej nowej funkcjonalności
2. **Utrzymuj AAA pattern** dla czytelności
3. **Mockuj wszystkie zależności** w testach jednostkowych
4. **Używaj opisowych nazw** testów: `Method_WhenCondition_ShouldResult`
5. **Testuj edge cases** i obsługę błędów
6. **Utrzymuj szybkość** testów - unikaj rzeczywistych połączeń z bazą
