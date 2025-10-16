using FantasyCoachAI.Domain.Interfaces;
using FantasyCoachAI.Infrastructure;
using FantasyCoachAI.Infrastructure.Configuration;
using FantasyCoachAI.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FantasyCoachAI.Infrastructure.Tests.Configuration
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void AddInfrastructure_WhenValidConfiguration_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateValidConfiguration();

            // Act
            services.AddInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<SupabaseSettings>().Should().NotBeNull();
            serviceProvider.GetService<Supabase.Client>().Should().NotBeNull();
            serviceProvider.GetService<ITeamRepository>().Should().BeOfType<TeamRepository>();
        }

        [Fact]
        public void AddInfrastructure_WhenMissingUrl_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateInvalidConfiguration(missingUrl: true);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(configuration));
            exception.Message.Should().Be("Supabase configuration is missing or incomplete");
        }

        [Fact]
        public void AddInfrastructure_WhenMissingKey_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateInvalidConfiguration(missingKey: true);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(configuration));
            exception.Message.Should().Be("Supabase configuration is missing or incomplete");
        }

        [Fact]
        public void AddInfrastructure_ShouldRegisterSupabaseClientAsScoped()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateValidConfiguration();

            // Act
            services.AddInfrastructure(configuration);

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(Supabase.Client));
            descriptor.Should().NotBeNull();
            descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }

        [Fact]
        public void AddInfrastructure_ShouldRegisterTeamRepositoryAsScoped()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateValidConfiguration();

            // Act
            services.AddInfrastructure(configuration);

            // Assert
            var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITeamRepository));
            descriptor.Should().NotBeNull();
            descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
            descriptor.ImplementationType.Should().Be(typeof(TeamRepository));
        }

        private static IConfiguration CreateValidConfiguration()
        {
            var configurationData = new Dictionary<string, string?>
            {
                {"Supabase:Url", "https://test.supabase.co"},
                {"Supabase:Key", "test-key"},
                {"Supabase:JwtSecret", "test-secret"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();
        }

        private static IConfiguration CreateInvalidConfiguration(bool missingUrl = false, bool missingKey = false)
        {
            var configurationData = new Dictionary<string, string?>();

            if (!missingUrl)
                configurationData["Supabase:Url"] = "https://test.supabase.co";
            
            if (!missingKey)
                configurationData["Supabase:Key"] = "test-key";
            
            configurationData["Supabase:JwtSecret"] = "test-secret";

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();
        }
    }
}
