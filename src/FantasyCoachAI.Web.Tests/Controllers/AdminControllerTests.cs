using Microsoft.AspNetCore.Mvc;
using FantasyCoachAI.Web.Controllers;
using FluentAssertions;
using Xunit;

namespace FantasyCoachAI.Web.Tests.Controllers
{
    [Trait("Category", "Unit")]
    public class AdminControllerTests
    {
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _controller = new AdminController();
        }

        #region GetDashboard Tests

        [Fact]
        public void GetDashboard_ShouldReturnOkWithDashboardData()
        {
            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var dashboard = okResult.Value.Should().BeOfType<dynamic>().Subject;

            // Verify the structure of the dashboard data
            ((dynamic)dashboard.data_quality).Should().NotBeNull();
            ((dynamic)dashboard.users).Should().NotBeNull();
            ((dynamic)dashboard.ai_performance).Should().NotBeNull();
            ((dynamic)dashboard.system).Should().NotBeNull();
        }

        [Fact]
        public void GetDashboard_ShouldReturnExpectedDataQualityMetrics()
        {
            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var dashboard = okResult.Value.Should().BeOfType<dynamic>().Subject;
            var dataQuality = ((dynamic)dashboard.data_quality);

            ((dynamic)dataQuality.freshness).Should().NotBeNull();
            ((string)((dynamic)dataQuality.freshness).status).Should().Be("good");
            ((string)((dynamic)dataQuality.freshness).last_update).Should().Be("2025-10-16T06:00:00Z");
            ((int)((dynamic)dataQuality.freshness).age_hours).Should().Be(2);
            ((double)dataQuality.completeness).Should().Be(0.98);
            ((double)dataQuality.scraping_success_rate).Should().Be(0.97);
        }

        [Fact]
        public void GetDashboard_ShouldReturnExpectedUserMetrics()
        {
            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var dashboard = okResult.Value.Should().BeOfType<dynamic>().Subject;
            var users = ((dynamic)dashboard.users);

            ((int)users.total_registered).Should().Be(156);
            ((int)users.monthly_active).Should().Be(89);
            ((int)users.new_last_7_days).Should().Be(12);
            ((double)users.retention_rate).Should().Be(0.67);
        }

        [Fact]
        public void GetDashboard_ShouldReturnExpectedAIPerformanceMetrics()
        {
            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var dashboard = okResult.Value.Should().BeOfType<dynamic>().Subject;
            var aiPerformance = ((dynamic)dashboard.ai_performance);

            ((double)aiPerformance.acceptance_rate).Should().Be(0.78);
            ((double)aiPerformance.success_rate).Should().Be(0.82);
            ((double)aiPerformance.average_points).Should().Be(59.2);
            ((double)aiPerformance.api_cost_month).Should().Be(45.67);
        }

        [Fact]
        public void GetDashboard_ShouldReturnExpectedSystemMetrics()
        {
            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var dashboard = okResult.Value.Should().BeOfType<dynamic>().Subject;
            var system = ((dynamic)dashboard.system);

            ((int)system.api_calls_today).Should().Be(2341);
            ((int)system.average_response_time).Should().Be(245);
            ((double)system.error_rate).Should().Be(0.012);
            ((double)system.uptime).Should().Be(0.9987);
        }

        #endregion
    }
}
