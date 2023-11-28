using Microsoft.AspNetCore.Mvc.Testing;
using TennisBookings.Mercandise.Api.IntegrationTests.Models;
using TennisBookings.Merchandise.Api;

namespace TennisBookings.Mercandise.Api.IntegrationTests.Controllers
{
    public class StockControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        public StockControllerTests(WebApplicationFactory<Startup> factory) { 
            _client = factory.CreateDefaultClient(new Uri("http://localhost/api/stock/"));
        }

        [Fact]
        public async Task GetStockTotal_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("total");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetStockTotal_ReturnsExpectedJsonContentString()
        {
            var response = await _client.GetStringAsync("total");

            Assert.Equal("{\"stockItemTotal\":100}", response);
        }

        [Fact]
        public async Task GetStockTotal_ReturnsExpectedJsonContentType()
        {
            var response = await _client.GetAsync("total");

            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task GetStockTotal_ReturnsExpectedJson() {
            var model = await _client.GetFromJsonAsync<ExpectedStockTotalOutputModel>("total");

            Assert.NotNull(model);
            Assert.True(model.StockItemTotal > 0);
        }
    }
}
