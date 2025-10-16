using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FantasyCoachAI.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  // Requires admin role (future Supabase Auth integration)
public class AdminController : ControllerBase
{
    /// <summary>
    /// Get system health dashboard (admin only)
    /// </summary>
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        // Placeholder data from API plan - in production, fetch from services/DB
        var dashboard = new
        {
            data_quality = new
            {
                freshness = new
                {
                    status = "good",
                    last_update = "2025-10-16T06:00:00Z",
                    age_hours = 2
                },
                completeness = 0.98,
                scraping_success_rate = 0.97
            },
            users = new
            {
                total_registered = 156,
                monthly_active = 89,
                new_last_7_days = 12,
                retention_rate = 0.67
            },
            ai_performance = new
            {
                acceptance_rate = 0.78,
                success_rate = 0.82,
                average_points = 59.2,
                api_cost_month = 45.67
            },
            system = new
            {
                api_calls_today = 2341,
                average_response_time = 245,
                error_rate = 0.012,
                uptime = 0.9987
            }
        };

        return Ok(dashboard);
    }
}
