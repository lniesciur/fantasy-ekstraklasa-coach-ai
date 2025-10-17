using System.ComponentModel;

namespace FantasyCoachAI.Domain.Enums
{
    public enum GameweekStatus
    {
        [Description("Nadchodząca")]
        Upcoming,

        [Description("Bieżąca")]
        Current,

        [Description("Zakończona")]
        Completed
    }
}
