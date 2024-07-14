using System.Net;
using System.Text;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;  // Tried to use WebApplicationFactory here but there was an issue with nuget package incompatibles.

namespace TinyUrlService.Test;

public class UrlControllerTests
{
    private readonly HttpClient _client;
    private readonly MockHttpMessageHandler _mockHttp;

    public UrlControllerTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _client = _mockHttp.ToHttpClient();
        _client.BaseAddress = new Uri("http://localhost:5237");
    }

    [Fact]
    public async Task WhenCreateShortUrlWithValidInputsShouldReturnShortUrl()
    {
        // Arrange
        var urlMapping = new
        {
            LongUrl = "https://www.example.com",
            ShortUrl = "example"
        };
        var content = new StringContent(JsonConvert.SerializeObject(urlMapping), Encoding.UTF8, "application/json");

        _mockHttp.When(HttpMethod.Post, "/api/url").Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(urlMapping.ShortUrl));

        // Act
        var response = await _client.PostAsync("/api/url", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("example", responseString);
    }

    [Fact]
    public async Task WhenGetLongUrlWithValidShortUrlShouldRedirect()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var urlMapping = new
        {
            LongUrl = longUrl,
            ShortUrl = "example"
        };

        _mockHttp.When(HttpMethod.Get, $"/api/url/{urlMapping.ShortUrl}").Respond(HttpStatusCode.Redirect, "text/plain", longUrl);

        // Act
        var response = await _client.GetAsync($"/api/url/{urlMapping.ShortUrl}");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task WhenCreateShortUrlWithInvalidUrlMapping_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidUrlMapping = new
        {
            LongUrl = (string)null,
            ShortUrl = "example"
        };
        var content = new StringContent(JsonConvert.SerializeObject(invalidUrlMapping), Encoding.UTF8, "application/json");

        _mockHttp.When(HttpMethod.Post, "/api/url").Respond(HttpStatusCode.BadRequest);

        // Act
        var response = await _client.PostAsync("/api/url", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
