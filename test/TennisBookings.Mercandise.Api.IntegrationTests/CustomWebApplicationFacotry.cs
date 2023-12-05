using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using TennisBookings.Mercandise.Api.IntegrationTests.Fakes;
using TennisBookings.Merchandise.Api.Diagnostics;
using TennisBookings.Merchandise.Api.External.Database;
using TennisBookings.Merchandise.Api.IntegrationTests.Fakes;

namespace TennisBookings.Mercandise.Api.IntegrationTests
{
    public class CustomWebApplicationFacotry<TStartUp> : WebApplicationFactory<TStartUp> 
        where TStartUp : class
    {
        public  FakeCloudDatabase FakeCloudDatabase { get; }

        public CustomWebApplicationFacotry()
        {
            FakeCloudDatabase = FakeCloudDatabase.WithDefaultProducts();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<ICloudDatabase, FakeCloudDatabase>();
                services.AddSingleton<IMetricRecorder, FakeMetricRecorder>();
            });
        }
    }
}
