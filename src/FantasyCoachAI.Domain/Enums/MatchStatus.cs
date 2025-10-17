using System.ComponentModel;

namespace FantasyCoachAI.Domain.Enums
{
    public enum MatchStatus
    {
        [Description("Zaplanowany")]
        Scheduled,

        [Description("Na żywo")]
        Live,

        [Description("Zakończony")]
        Finished,

        [Description("Przełożony")]
        Postponed,

        [Description("Odwołany")]
        Cancelled
    }
}
