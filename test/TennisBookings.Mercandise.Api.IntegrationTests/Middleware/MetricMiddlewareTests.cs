using Microsoft.AspNetCore.TestHost;
using TennisBookings.Mercandise.Api.IntegrationTests.Fakes;
using TennisBookings.Merchandise.Api;
using TennisBookings.Merchandise.Api.Diagnostics;
using TennisBookings.Merchandise.Api.External.Database;
using TennisBookings.Merchandise.Api.IntegrationTests.Fakes;

namespace TennisBookings.Mercandise.Api.IntegrationTests.Middleware
{
    public class MetricMiddlewareTests: IClassFixture<CustomWebApplicationFacotry<Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFacotry<Startup> _factory;

        public MetricMiddlewareTests(CustomWebApplicationFacotry<Startup> factory)
        {
            _client = factory.CreateDefaultClient();
            _factory = factory;
        }

        [Fact]
        public async Task RequestForPage_ResultsInExpectedMetrics()
        {
            const string userAgent = "SomeProduct/1.0";

            var request1 = new HttpRequestMessage(HttpMethod.Get, "/does-not-exist");
            request1.Headers.TryAddWithoutValidation("User-Agent", userAgent);

            var request2 = new HttpRequestMessage(HttpMethod.Get, "/healthcheck");
            request2.Headers.TryAddWithoutValidation("User-Agent", userAgent);

            await _client.SendAsync(request1);
            await _client.SendAsync(request2);

            var metricRecorder = _factory.Services.GetRequiredService<IMetricRecorder>() as FakeMetricRecorder;

            var metric1 = metricRecorder.Metrics.FirstOrDefault();
            var metric2 = metricRecorder.Metrics.LastOrDefault();

            Assert.Equal("sending-response", metric1.Name);
            Assert.Equal(1, metric1.Increment);
            Assert.Equal("status_code:404", metric1.Tags[0]);
            Assert.Equal($"user_agent:{userAgent}", metric1.Tags[1]);


            Assert.Equal("sending-response", metric2.Name);
            Assert.Equal(1, metric2.Increment);
            Assert.Equal("status_code:200", metric2.Tags[0]);
            Assert.Equal($"user_agent:{userAgent}", metric2.Tags[1]);
        }

        [Fact]
        public async Task Exception_ResultsInExpectedMetric()
        {
            const string userAgent = "SomeProduct/1.0";
            const string correlationId = "ABC123";

            var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<ICloudDatabase>(new FakeCloudDatabase() { ShouldThrow = true });
                });
            });

            await factory.Server.CreateRequest("api/products")
                .AddHeader("User-Agent", userAgent)
                .AddHeader("Correlation-Id", correlationId)
                .GetAsync();

            var metricRecorder = factory.Services.GetRequiredService<IMetricRecorder>() as FakeMetricRecorder;

            var metric1 = metricRecorder.Metrics.FirstOrDefault();

            Assert.Equal("unhandled-exception", metric1.Name);
            Assert.Equal($"correlation_id:{correlationId}", metric1.Tags[0]);
            Assert.Equal($"user_agent:{userAgent}", metric1.Tags[01]);
        }
    }
}
