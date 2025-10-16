using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace FantasyCoachAI.Infrastructure.Persistence.SupabaseModels
{
    [Table("teams")]
    public class TeamDbModel : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("short_code")]
        public string? ShortCode { get; set; }

        [Column("crest_url")]
        public string? CrestUrl { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
