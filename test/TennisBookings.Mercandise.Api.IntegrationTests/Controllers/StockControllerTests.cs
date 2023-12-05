using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using TennisBookings.Mercandise.Api.IntegrationTests.Fakes;
using TennisBookings.Mercandise.Api.IntegrationTests.Models;
using TennisBookings.Merchandise.Api;
using TennisBookings.Merchandise.Api.Data.Dto;
using TennisBookings.Merchandise.Api.External.Database;

namespace TennisBookings.Mercandise.Api.IntegrationTests.Controllers
{
    public class StockControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private HttpClient _client;
        public StockControllerTests(WebApplicationFactory<Startup> factory) { 
            factory.ClientOptions.BaseAddress = new Uri("http://localhost/api/stock/");
            _client = factory.CreateClient();
            _factory = factory;
        }

        //[Fact]
        //public async Task GetStockTotal_ReturnsSuccessStatusCode()
        //{
        //    var response = await _client.GetAsync("total");

        //    response.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task GetStockTotal_ReturnsExpectedJsonContentString()
        //{
        //    var response = await _client.GetStringAsync("total");

        //    Assert.Equal("{\"stockItemTotal\":100}", response);
        //}

        //[Fact]
        //public async Task GetStockTotal_ReturnsExpectedJsonContentType()
        //{
        //    var response = await _client.GetAsync("total");

        //    Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
        //}

        [Fact]
        public async Task GetStockTotal_ReturnsExpectedJson() {
            var model = await _client.GetFromJsonAsync<ExpectedStockTotalOutputModel>("total");

            Assert.NotNull(model);
            Assert.True(model.StockItemTotal > 0);
        }

        [Fact]
        public async Task GetStockTotal_ReturnsExpectedStockQuantity()
        {
            var cloudDatabase = new FakeCloudDatabase(new[] { 
                new ProductDto() {  StockCount = 200 },
                new ProductDto() {  StockCount = 500 },
                new ProductDto() {  StockCount = 300 }
            });

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services => services.AddSingleton<ICloudDatabase>(cloudDatabase));
            }).CreateClient();

            var model = await client.GetFromJsonAsync<ExpectedStockTotalOutputModel>("total");

            Assert.Equal(1000, model.StockItemTotal);
        }
    }
}
