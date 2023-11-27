using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TennisBookings.Mercandise.Api.IntegrationTests.Models;
using TennisBookings.Merchandise.Api;


namespace TennisBookings.Mercandise.Api.IntegrationTests.Controllers
{
    public class CategoriesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        public CategoriesControllerTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.BaseAddress = new Uri("http://localhost/api/categories");
            _client = factory.CreateClient();
        }

      
        [Fact]
        public async Task GetAll_ReturnsExpectedResponse()
        {
            List<string> expected = new List<string> { "Accessories", "Bags",
                            "Balls",
                            "Clothing",
                            "Rackets" };

            
            var model = await _client.GetFromJsonAsync<ExpectedCategoriesModel>("");
            
            Assert.NotNull(model?.AllowedCategories);
            Assert.Equal(expected.OrderBy(x => x), model.AllowedCategories.OrderBy(x => x));
        }

        [Fact]
        public async Task GetAll_SetsExpectedCAcheControlHeader()
        {
            var response = await _client.GetAsync("");

            var header = response.Headers.CacheControl;

            Assert.True(header.MaxAge.HasValue);
            Assert.Equal(TimeSpan.FromMinutes(5), header.MaxAge);
            Assert.True(header.Public);
        }
    }    
}
