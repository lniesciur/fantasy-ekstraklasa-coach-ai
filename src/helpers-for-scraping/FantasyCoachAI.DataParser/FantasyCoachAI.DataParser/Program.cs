using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FantasyCoachAI.DataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new PlayerStatsHtmlParser();

            // Wczytaj plik ze statystykami z embedded resource
            string htmlContent = parser.LoadEmbeddedResource("FantasyCoachAI.DataParser.player_stats.html");

            // Wczytaj plik z cenami zawodników
            string playersHtml = parser.LoadEmbeddedResource("FantasyCoachAI.DataParser.players.html");

            // Parsuj dane
            List<PlayerStats> players = parser.ParseHtml(htmlContent);

            // Parsuj ceny zawodników
            Dictionary<string, decimal> playerPrices = parser.ParsePlayerPrices(playersHtml);

            Console.WriteLine($"✓ Wczytano {players.Count} rekordów ze statystykami zawodników");
            Console.WriteLine($"✓ Wczytano {playerPrices.Count} cen zawodników\n");

            // Generuj SQL
            try
            {
                string sqlScript = parser.GenerateSqlScript(players);

                // Zapisz do pliku
                string currentDir = Directory.GetCurrentDirectory();
                string filePath = Path.Combine(currentDir, "players_stats_seed.sql");
                File.WriteAllText(filePath, sqlScript, Encoding.UTF8);
                Console.WriteLine($"Zapisano plik do: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas generowania SQL: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return;
            }

            Console.WriteLine($"✓ Wygenerowano skrypt SQL dla {players.Count} zawodników.");
            Console.WriteLine("✓ Plik zapisany jako: players_stats_seed.sql\n");

            // Generuj SQL seed dla zawodników
            var playersSeedSql = parser.GeneratePlayersSeedSql(players, playerPrices);
            File.WriteAllText("players_seed.sql", playersSeedSql, Encoding.UTF8);

            Console.WriteLine("✓ Wygenerowano SQL seed dla zawodników.");
            Console.WriteLine("✓ Plik zapisany jako: players_seed.sql\n");

            // Diagnostyka
            parser.PrintDiagnostics(players, playerPrices);
        }
    }

    public class PlayerStats
    {
        public string? PlayerId { get; set; }
        public string? NazwiskoImie { get; set; }
        public string? Pozycja { get; set; }
        public string? Klub { get; set; }
        public int Minuty { get; set; }
        public int Bramki { get; set; }
        public int Asysty { get; set; }
        public int AsystyLotto { get; set; }
        public int BramkiSamobojcze { get; set; }
        public int KarneWykorzystane { get; set; }
        public int KarneWywalczone { get; set; }
        public int KarneSpowodowane { get; set; }
        public int KarneZmarnowane { get; set; }
        public int KarneObronione { get; set; }
        public int JedenastkaKolejki { get; set; }
        public int StrzalyObronione { get; set; }
        public int ZolteKartki { get; set; }
        public int CzerwoneKartki { get; set; }
        public int Punkty { get; set; }
    }

    public class PlayerStatsHtmlParser
    {

        private readonly Dictionary<string, string> _positionMapping = new Dictionary<string, string>
        {
            {"Bramkarz", "GK"},
            {"Obrońca", "DEF"},
            {"Pomocnik", "MID"},
            {"Napastnik", "FWD"}
        };

        public string LoadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Nie znaleziono zasobu: {resourceName}");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public List<PlayerStats> ParseHtml(string html)
        {
            var players = new List<PlayerStats>();

            // Regex do wyciągnięcia wierszy tabeli
            var rowPattern = @"<tr[^>]*>\s*<td[^>]*>.*?</td>\s*(?:<td[^>]*>.*?</td>\s*){17}</tr>";
            var rowMatches = Regex.Matches(html, rowPattern, RegexOptions.Singleline);

            foreach (Match rowMatch in rowMatches)
            {
                string row = rowMatch.Value;

                // Pomijamy wiersz nagłówka
                if (row.Contains("Nazwisko i imię"))
                    continue;

                // Wyciągnij ID piłkarza z onclick="app.Player.info('2225');"
                var playerIdMatch = Regex.Match(row, @"app\.Player\.info\('(\d+)'\)");
                string playerId = playerIdMatch.Success ? playerIdMatch.Groups[1].Value : "";

                var cells = ExtractCells(row);

                if (cells.Count >= 18)
                {
                    var player = new PlayerStats
                    {
                        PlayerId = playerId,
                        NazwiskoImie = cells[0],
                        Pozycja = cells[1],
                        Klub = cells[2],
                        Minuty = ParseInt(cells[3]),
                        Bramki = ParseInt(cells[4]),
                        Asysty = ParseInt(cells[5]),
                        AsystyLotto = ParseInt(cells[6]),
                        BramkiSamobojcze = ParseInt(cells[7]),
                        KarneWykorzystane = ParseInt(cells[8]),
                        KarneWywalczone = ParseInt(cells[9]),
                        KarneSpowodowane = ParseInt(cells[10]),
                        KarneZmarnowane = ParseInt(cells[11]),
                        KarneObronione = ParseInt(cells[12]),
                        JedenastkaKolejki = ParseInt(cells[13]),
                        StrzalyObronione = ParseInt(cells[14]),
                        ZolteKartki = ParseInt(cells[15]),
                        CzerwoneKartki = ParseInt(cells[16]),
                        Punkty = ParseInt(cells[17])
                    };

                    players.Add(player);
                }
            }

            return players;
        }

        private List<string> ExtractCells(string row)
        {
            var cells = new List<string>();
            var cellPattern = @"<td[^>]*>(.*?)</td>";
            var cellMatches = Regex.Matches(row, cellPattern, RegexOptions.Singleline);

            foreach (Match cellMatch in cellMatches)
            {
                string cellContent = cellMatch.Groups[1].Value;

                // Wyciągnij nazwisko z linku lub zwykły tekst
                var nameMatch = Regex.Match(cellContent, @">([^<]+)</a>");
                if (nameMatch.Success)
                {
                    cells.Add(CleanText(nameMatch.Groups[1].Value));
                }
                else
                {
                    cells.Add(CleanText(cellContent));
                }
            }

            return cells;
        }

        private string CleanText(string text)
        {
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private int ParseInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            int result;
            return int.TryParse(value, out result) ? result : 0;
        }

        public string GenerateSqlScript(List<PlayerStats> players)
        {
            var sb = new StringBuilder();

            // Nagłówek
            sb.AppendLine("-- Skrypt SQL dla statystyk zawodników");
            sb.AppendLine("-- Wygenerowano automatycznie");
            sb.AppendLine("-- UWAGA: Ten skrypt wymaga wcześniejszego wypełnienia tabel: players, gameweeks, matches");
            sb.AppendLine("-- UWAGA: Ten skrypt wymaga funkcji insert_player_stats_safe() zdefiniowanej w migracji 20251019000000_add_player_stats_security.sql\n");

            // Disable trigger during seeding
            sb.AppendLine("-- Disable trigger during seeding to avoid constraint violations");
            sb.AppendLine("ALTER TABLE player_stats DISABLE TRIGGER check_player_exists_trigger;\n");

            // SELECT statements dla player_stats używające insert_player_stats_safe()
            sb.AppendLine("-- Wstawianie statystyk zawodników za pomocą insert_player_stats_safe()\n");

            foreach (var player in players)
            {
                // Mapuj pozycję na kod z bazy danych
                string positionCode = _positionMapping.ContainsKey(player.Pozycja ?? "") 
                    ? _positionMapping[player.Pozycja ?? ""] 
                    : "MID"; // domyślnie MID jeśli pozycja nieznana

                // Mapuj nazwy klubów
                string mappedClubName = MapClubName(player.Klub ?? "");

                // Mapuj punkty fantasy
                int fantasyPoints = CalculateFantasyPoints(player);

                // Bezpieczne wartości boolowskie dla SQL
                string predictedStart = "false";
                string inTeamOfWeek = player.JedenastkaKolejki > 0 ? "true" : "false";

                sb.AppendLine($"SELECT insert_player_stats_safe(");
                sb.AppendLine($"    '{EscapeSql(player.NazwiskoImie ?? "")}',");
                sb.AppendLine($"    {player.Minuty},");
                sb.AppendLine($"    {player.Bramki},");
                sb.AppendLine($"    {player.Asysty},");
                sb.AppendLine($"    {player.ZolteKartki},");
                sb.AppendLine($"    {player.CzerwoneKartki},");
                sb.AppendLine($"    {predictedStart},");
                sb.AppendLine($"    {player.AsystyLotto},");
                sb.AppendLine($"    {player.BramkiSamobojcze},");
                sb.AppendLine($"    {player.KarneObronione},");
                sb.AppendLine($"    {player.KarneWywalczone},");
                sb.AppendLine($"    {inTeamOfWeek},");
                sb.AppendLine($"    {player.StrzalyObronione},");
                sb.AppendLine($"    {player.KarneWykorzystane},");
                sb.AppendLine($"    {player.KarneSpowodowane},");
                sb.AppendLine($"    {player.KarneZmarnowane},");
                sb.AppendLine($"    'Pewny',");
                sb.AppendLine($"    {fantasyPoints}");
                sb.AppendLine($");");
                sb.AppendLine();
            }

            // Re-enable trigger after seeding
            sb.AppendLine("-- Re-enable trigger after seeding");
            sb.AppendLine("ALTER TABLE player_stats ENABLE TRIGGER check_player_exists_trigger;");

            return sb.ToString();
        }

        private string EscapeSql(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("'", "''");
        }

        private int CalculateFantasyPoints(PlayerStats player)
        {
            int points = 0;

            // Bramki (różne punkty w zależności od pozycji)
            string position = player.Pozycja ?? "";
            int goalPoints = position switch
            {
                "Bramkarz" => player.Bramki * 6,
                "Obrońca" => player.Bramki * 6,
                "Pomocnik" => player.Bramki * 5,
                "Napastnik" => player.Bramki * 4,
                _ => player.Bramki * 5
            };
            points += goalPoints;

            // Asysty
            points += player.Asysty * 3;

            // Asysty Lotto
            points += player.AsystyLotto * 3;

            // Bramki samobójcze (ujemne punkty)
            points += player.BramkiSamobojcze * -2;

            // Karne wykorzystane
            points += player.KarneWykorzystane * 2;

            // Karne wywalczone
            points += player.KarneWywalczone * 1;

            // Karne spowodowane (ujemne punkty)
            points += player.KarneSpowodowane * -1;

            // Karne zmarnowane (ujemne punkty)
            points += player.KarneZmarnowane * -2;

            // Karne obronione (dla bramkarzy)
            if (position == "Bramkarz")
            {
                points += player.KarneObronione * 5;
            }

            // Jedenastka kolejki
            points += player.JedenastkaKolejki * 1;

            // Strzały obronione (dla bramkarzy)
            if (position == "Bramkarz")
            {
                points += player.StrzalyObronione * 1;
            }

            // Żółte kartki (ujemne punkty)
            points += player.ZolteKartki * -1;

            // Czerwone kartki (ujemne punkty)
            points += player.CzerwoneKartki * -3;

            // Punkty za minuty (jeśli grał więcej niż 60 minut)
            if (player.Minuty >= 60)
            {
                points += 2;
            }
            else if (player.Minuty > 0)
            {
                points += 1;
            }

            return points;
        }

        private string MapClubName(string clubName)
        {
            // Mapuj nazwy klubów z parsera na nazwy z bazy danych
            switch (clubName)
            {
                case "Bruk-Bet Termalica Nieciecza":
                    return "Termalica B-B";
                default:
                    return clubName; // Jeśli nie ma mapowania, zwróć oryginalną nazwę
            }
        }

        public string GeneratePlayersSeedSql(List<PlayerStats> players, Dictionary<string, decimal> playerPrices)
        {
            var sb = new StringBuilder();

            // Nagłówek
            sb.AppendLine("-- SQL seed dla tabeli players");
            sb.AppendLine("-- Wygenerowano automatycznie\n");

            // Używamy HashSet do filtrowania duplikatów
            var uniquePlayers = new HashSet<string>();
            var sortedPlayers = new List<(string? PlayerId, string? Name, string? Position, string? Club, decimal Price)>();

            foreach (var player in players)
            {
                string key = $"{player.PlayerId}|{player.NazwiskoImie}|{player.Pozycja}|{player.Klub}";

                if (!uniquePlayers.Contains(key))
                {
                    uniquePlayers.Add(key);

                    // Szukaj ceny po ID piłkarza
                    decimal price = 0;
                    if (!string.IsNullOrEmpty(player.PlayerId) && playerPrices.ContainsKey(player.PlayerId))
                    {
                        price = playerPrices[player.PlayerId];
                    }

                    sortedPlayers.Add((player.PlayerId, player.NazwiskoImie, player.Pozycja, player.Klub, price));
                }
            }

            // Sortowanie alfabetyczne po nazwisku
            sortedPlayers.Sort((a, b) => (a.Name ?? "").CompareTo(b.Name ?? ""));

            // Generowanie INSERT statements
            sb.AppendLine("INSERT INTO players (name, team_id, position, price, health_status) VALUES");
            
            var insertValues = new List<string>();
            int foundPrices = 0;
            int skippedPlayers = 0;

            foreach (var player in sortedPlayers)
            {
                // Sprawdź czy mamy cenę
                if (player.Price <= 0)
                {
                    skippedPlayers++;
                    continue;
                }

                // Mapuj pozycję
                string positionCode = _positionMapping.ContainsKey(player.Position ?? "") 
                    ? _positionMapping[player.Position ?? ""] 
                    : "MID"; // domyślnie MID jeśli pozycja nieznana

                string playerName = EscapeSql(player.Name ?? "");
                string clubName = EscapeSql(player.Club ?? "");
                string healthStatus = "Pewny"; // domyślny status zdrowia

                // Mapuj nazwy klubów z parsera na nazwy z bazy danych
                string mappedClubName = MapClubName(clubName);
                
                // Użyj subquery do znalezienia team_id na podstawie nazwy klubu
                string teamIdSubquery = $"(SELECT id FROM teams WHERE name LIKE '%{mappedClubName}%' LIMIT 1)";

                insertValues.Add($"('{playerName}', {teamIdSubquery}, '{positionCode}', {player.Price.ToString("F2").Replace(',', '.')}, '{healthStatus}')");
                foundPrices++;
            }

            // Połącz wszystkie wartości z przecinkami
            sb.AppendLine(string.Join(",\n", insertValues));
            sb.AppendLine(";");

            // Dodaj statystyki
            sb.AppendLine();
            sb.AppendLine($"-- Statystyki:");
            sb.AppendLine($"-- Łącznie unikalnych zawodników: {sortedPlayers.Count}");
            sb.AppendLine($"-- Wstawiono do bazy: {foundPrices}");
            sb.AppendLine($"-- Pominięto (brak ceny): {skippedPlayers}");

            return sb.ToString();
        }

        public Dictionary<string, decimal> ParsePlayerPrices(string html)
        {
            var prices = new Dictionary<string, decimal>();

            // Regex do wyciągnięcia cały wiersz z data-player-price i data-info-id
            // Szukamy całego wiersza <tr> od początku do końca </tr>
            var rowPattern = @"<tr\s+[^>]*data-player-price=""([^""]*?)""[^>]*>.*?data-info-id=""(\d+)"".*?</tr>";
            var matches = Regex.Matches(html, rowPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                if (match.Groups.Count >= 3)
                {
                    string priceStr = match.Groups[1].Value.Trim();
                    string playerId = match.Groups[2].Value.Trim();

                    decimal price;
                    if (decimal.TryParse(priceStr.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out price))
                    {
                        if (!prices.ContainsKey(playerId))
                        {
                            prices[playerId] = price;
                        }
                    }
                }
            }

            return prices;
        }

        public void PrintDiagnostics(List<PlayerStats> players, Dictionary<string, decimal> playerPrices)
        {
            Console.WriteLine("=== Diagnostyka ===");
            Console.WriteLine($"Wczytano {players.Count} rekordów ze statystykami.");
            Console.WriteLine($"Wczytano {playerPrices.Count} cen zawodników.");

            int matchedPrices = 0;
            int unmatchedPlayers = 0;

            foreach (var player in players)
            {
                if (!string.IsNullOrEmpty(player.PlayerId) && playerPrices.ContainsKey(player.PlayerId))
                {
                    matchedPrices++;
                }
                else
                {
                    unmatchedPlayers++;
                }
            }

            Console.WriteLine($"Ceny dopasowane: {matchedPrices}");
            Console.WriteLine($"Ceny nie dopasowane: {unmatchedPlayers}");
            Console.WriteLine($"Łącznie: {players.Count}");
        }
    }
}