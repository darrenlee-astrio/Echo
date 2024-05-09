using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;

namespace Echo.Api.Tests.Integration;

public class ApiTests : IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly HttpClient _httpClient;

    public ApiTests(WebApplicationFactory<IApiMarker> factory)
    {
        _httpClient = factory.CreateClient();
    }

    private HttpRequestMessage CreateHttpRequestMessage(string method, string uri, string requestBody, string mediaType)
    {
        var message = new HttpRequestMessage(new HttpMethod(method), uri);
        message.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
        return message;
    }

    [Theory]
    [InlineData("GET", "/api/home", "", "application/json")]
    [InlineData("POST", "/api/random", "{\"name\":\"John Doe\",\"age\":30,\"email\":\"john.doe@example.com\",\"address\":{\"street\":\"123 Main St\",\"city\":\"Anytown\",\"country\":\"USA\"},\"is_student\":false,\"interests\":[\"reading\",\"coding\",\"traveling\"]}", "application/json")]
    [InlineData("POST", "/api/help", "Hello world", "text/plain")]
    [InlineData("DELETE", "/api/products", "<Id>1</Id>", "application/xml")]
    public async void Api_ShouldReturnResponseSameAsRequest_WhenRouteContainsApi(string method, string uri, string requestBody, string mediaType)
    {
        // Arrange
        var message = CreateHttpRequestMessage(method, uri, requestBody, mediaType);

        // Act
        var response = await _httpClient.SendAsync(message);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be(mediaType);
        content.Should().Be(requestBody);
    }

    [Fact]
    public async void Api_ShouldReturnHelloWorld_WhenInvokingHelloEndpoint()
    {
        // Arrange
        var message = CreateHttpRequestMessage("GET", "/hello", string.Empty, "text/plain");

        // Act
        var response = await _httpClient.SendAsync(message);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
        content.Should().Be("Hello world");
    }

    [Fact]
    public async void Api_ShouldReturnNotFound_WhenInvokingNonexistentEndpointWithoutApiPath()
    {
        // Arrange
        var message = CreateHttpRequestMessage("POST", "/status", "abcdefg", "text/plain");

        // Act
        var response = await _httpClient.SendAsync(message);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
