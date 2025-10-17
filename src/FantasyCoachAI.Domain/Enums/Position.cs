using System.ComponentModel;

namespace FantasyCoachAI.Domain.Enums
{
    public enum Position
    {
        [Description("Bramkarz")]
        GK,

        [Description("Obrońca")]
        DEF,

        [Description("Pomocnik")]
        MID,

        [Description("Napastnik")]
        FWD
    }
}
