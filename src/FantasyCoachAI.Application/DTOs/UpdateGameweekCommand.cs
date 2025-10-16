namespace FantasyCoachAI.Application.DTOs
{
    public class UpdateGameweekCommand
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
