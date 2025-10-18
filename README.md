# Fantasy Ekstraklasa Coach AI

Aplikacja webowa do zarządzania fantasy football dla polskiej Ekstraklasy, zbudowana w technologii .NET 8 i Blazor Server. Aplikacja umożliwia śledzenie drużyn, meczów, kolejek oraz statystyk zawodników w czasie rzeczywistym.

## 🚀 Funkcjonalności

- **Zarządzanie drużynami** - dodawanie, edycja i wyświetlanie drużyn Ekstraklasy
- **System kolejek** - organizacja meczów w kolejki sezonu
- **Mecze** - śledzenie wyników, statusów i harmonogramów meczów
- **Statystyki zawodników** - szczegółowe statystyki z automatycznym importem
- **API REST** - pełne API do integracji z zewnętrznymi systemami
- **Dashboard administracyjny** - panel do zarządzania danymi
- **Health checks** - monitoring stanu aplikacji i połączenia z bazą danych
- **System logowania** - forma logowania z mockowaną autentykacją (przygotowana do integracji z Supabase)

## 🏗️ Architektura

Aplikacja została zbudowana zgodnie z zasadami **Clean Architecture**:

```
src/
├── FantasyCoachAI.Domain/          # Warstwa domenowa
│   ├── Entities/                   # Encje biznesowe
│   ├── Enums/                      # Wyliczenia
│   ├── Interfaces/                 # Interfejsy domenowe
│   └── Exceptions/                 # Wyjątki domenowe
├── FantasyCoachAI.Application/     # Warstwa aplikacyjna
│   ├── DTOs/                       # Obiekty transferu danych
│   ├── Interfaces/                 # Interfejsy serwisów
│   ├── Services/                   # Logika biznesowa
│   └── Validators/                 # Walidatory FluentValidation
├── FantasyCoachAI.Infrastructure/ # Warstwa infrastruktury
│   ├── Persistence/               # Implementacje Supabase
│   ├── Repositories/              # Repozytoria danych
│   └── Mappers/                    # Mapowanie obiektów
└── FantasyCoachAI.Web/            # Warstwa prezentacji
    ├── Components/                # Komponenty Blazor
    ├── Controllers/               # Kontrolery API
    └── Middleware/                # Middleware aplikacji
```

## 🛠️ Technologie

- **.NET 8** - Framework aplikacji
- **Blazor Server** - Interfejs użytkownika
- **MudBlazor** - Komponenty UI
- **Supabase** - Baza danych PostgreSQL
- **FluentValidation** - Walidacja danych
- **Swagger/OpenAPI** - Dokumentacja API
- **xUnit** - Testy jednostkowe

## 📋 Wymagania

- .NET 8 SDK
- Supabase CLI (opcjonalnie, do lokalnego developmentu)
- Visual Studio 2022 lub VS Code

## 🚀 Instalacja i uruchomienie

### 1. Klonowanie repozytorium

```bash
git clone https://github.com/your-username/fantasy-ekstraklasa-coach-ai.git
cd fantasy-ekstraklasa-coach-ai
```

### 2. Konfiguracja bazy danych

#### Opcja A: Supabase Cloud (zalecane)

1. Utwórz projekt na [supabase.com](https://supabase.com)
2. Skopiuj URL i klucz API z dashboardu Supabase
3. Zaktualizuj `appsettings.json`:

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-anon-key"
  }
}
```

#### Opcja B: Supabase lokalnie

```bash
# Zainstaluj Supabase CLI
npm install -g supabase

# Uruchom lokalny Supabase
supabase start

# Zastosuj migracje
supabase db reset
```

### 3. Uruchomienie aplikacji

```bash
cd src
dotnet restore
dotnet build
dotnet run --project FantasyCoachAI.Web
```

Aplikacja będzie dostępna pod adresem:
- **Web UI**: https://localhost:5001
- **API**: https://localhost:5001/swagger
- **Health Check**: https://localhost:5001/health

## 🧪 Testy

```bash
# Uruchom wszystkie testy
dotnet test

# Uruchom testy z pokryciem
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 API Endpoints

### Drużyny
- `GET /api/teams` - Lista drużyn
- `GET /api/teams/{id}` - Szczegóły drużyny
- `POST /api/teams` - Utwórz drużynę
- `PUT /api/teams/{id}` - Aktualizuj drużynę
- `DELETE /api/teams/{id}` - Usuń drużynę

### Kolejki
- `GET /api/gameweeks` - Lista kolejek
- `GET /api/gameweeks/{id}` - Szczegóły kolejki
- `POST /api/gameweeks` - Utwórz kolejkę
- `PUT /api/gameweeks/{id}` - Aktualizuj kolejkę

### Mecze
- `GET /api/matches` - Lista meczów
- `GET /api/matches/{id}` - Szczegóły meczu
- `POST /api/matches` - Utwórz mecz
- `PUT /api/matches/{id}` - Aktualizuj mecz

### Admin
- `GET /api/admin/health` - Status systemu
- `POST /api/admin/import` - Import danych

## 🗄️ Struktura bazy danych

Aplikacja wykorzystuje PostgreSQL z następującymi głównymi tabelami:

- **teams** - Drużyny Ekstraklasy
- **gameweeks** - Kolejki sezonu
- **matches** - Mecze z wynikami
- **players** - Zawodnicy
- **player_stats** - Statystyki zawodników
- **import_logs** - Logi importu danych

## 🔧 Konfiguracja

### Zmienne środowiskowe

```bash
# Supabase
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_KEY=your-anon-key

# Logging
LOG_LEVEL=Information
```

### appsettings.json

```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "Key": "your-anon-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 🚀 Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FantasyCoachAI.Web/FantasyCoachAI.Web.csproj", "FantasyCoachAI.Web/"]
RUN dotnet restore "FantasyCoachAI.Web/FantasyCoachAI.Web.csproj"
COPY . .
WORKDIR "/src/FantasyCoachAI.Web"
RUN dotnet build "FantasyCoachAI.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FantasyCoachAI.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FantasyCoachAI.Web.dll"]
```

### Azure App Service

1. Utwórz App Service w Azure Portal
2. Skonfiguruj zmienne środowiskowe
3. Wdróż kod z GitHub Actions

## 🤝 Współtworzenie

1. Fork repozytorium
2. Utwórz branch dla nowej funkcjonalności (`git checkout -b feature/amazing-feature`)
3. Commit zmian (`git commit -m 'Add amazing feature'`)
4. Push do brancha (`git push origin feature/amazing-feature`)
5. Otwórz Pull Request

## 📝 Licencja

Ten projekt jest licencjonowany na licencji MIT - zobacz plik [LICENSE](LICENSE) dla szczegółów.

## 📞 Kontakt

- **Autor**: [Twoje Imię]
- **Email**: [twoj-email@example.com]
- **GitHub**: [@twoj-username](https://github.com/twoj-username)

## 🙏 Podziękowania

- [Supabase](https://supabase.com) za świetną platformę backend-as-a-service
- [MudBlazor](https://mudblazor.com) za komponenty UI
- [.NET](https://dotnet.microsoft.com) za framework aplikacji

---

**Uwaga**: To jest projekt w fazie rozwoju. Niektóre funkcjonalności mogą być jeszcze w trakcie implementacji.
