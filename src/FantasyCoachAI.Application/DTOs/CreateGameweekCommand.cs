namespace FantasyCoachAI.Application.DTOs
{
    public class CreateGameweekCommand
    {
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
