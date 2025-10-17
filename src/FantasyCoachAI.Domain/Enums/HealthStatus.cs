using System.ComponentModel;

namespace FantasyCoachAI.Domain.Enums
{
    public enum HealthStatus
    {
        [Description("Pewny")]
        Available,

        [Description("Kontuzjowany")]
        Injured,

        [Description("Wątpliwy")]
        Doubtful,

        [Description("Zawieszony")]
        Suspended
    }
}
