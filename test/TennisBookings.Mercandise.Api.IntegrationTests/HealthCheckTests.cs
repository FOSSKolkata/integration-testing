using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using TennisBookings.Merchandise.Api;
using Xunit;

namespace TennisBookings.Mercandise.Api.IntegrationTests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        public HealthCheckTests(WebApplicationFactory<Startup> factory)
        {
                _httpClient = factory.CreateDefaultClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _httpClient.GetAsync("/healthcheck");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
