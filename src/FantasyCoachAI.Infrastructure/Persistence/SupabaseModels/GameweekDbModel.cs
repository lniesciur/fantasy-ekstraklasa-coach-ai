using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FantasyCoachAI.Infrastructure.Persistence.SupabaseModels
{
    [Table("gameweeks")]
    public class GameweekDbModel : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public int Id { get; set; }

        [Column("number")]
        public int Number { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }
    }
}
