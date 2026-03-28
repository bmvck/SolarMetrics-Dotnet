using Xunit;

namespace SolarMetrics.IntegrationTests.Fixtures;

/// <summary>Compartilha uma única <see cref="SolarMetricsApiFactory"/> entre testes de integração (Collection Fixture xUnit).</summary>
[CollectionDefinition("ApiIntegration")]
public sealed class ApiIntegrationCollection : ICollectionFixture<SolarMetricsApiFactory>
{
}
