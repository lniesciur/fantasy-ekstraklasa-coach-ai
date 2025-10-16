namespace FantasyCoachAI.Application.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public int Pages { get; set; }

        public bool HasNextPage => Page < Pages;
        public bool HasPreviousPage => Page > 1;
        
        public static PaginationDto Create(int page, int limit, int total)
        {
            var pages = (int)Math.Ceiling((double)total / limit);
            return new PaginationDto
            {
                Page = page,
                Limit = limit,
                Total = total,
                Pages = pages
            };
        }
    }
}
