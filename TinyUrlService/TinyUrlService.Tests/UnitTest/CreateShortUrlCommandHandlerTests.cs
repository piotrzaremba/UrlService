using FluentValidation;
using FluentValidation.Results;
using Moq;
using TinyUrlService.Domain.Services;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Handlers;

namespace TinyUrlService.Tests;

// Create some sample tests.
// There is an issue with the ReturnAsync not returning the specified value, I will have to dig into this layer.
// Possibly an issue with the .net 6 version of Moq?
public class CreateShortUrlCommandHandlerTests
{
    private readonly CreateShortUrlHandler _handler;
    private readonly Mock<IUrlService> _urlServiceMock;
    private readonly Mock<IValidator<CreateShortUrlCommand>> _validatorMock;

    public CreateShortUrlCommandHandlerTests()
    {
        _urlServiceMock = new Mock<IUrlService>();
        _validatorMock = new Mock<IValidator<CreateShortUrlCommand>>();
        _handler = new CreateShortUrlHandler(_urlServiceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task WhenWithValidInputsShouldReturnShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl = "example";
        var command = new CreateShortUrlCommand { LongUrl = longUrl, ShortUrl = shortUrl };

        _validatorMock.Setup(x => x.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
        _urlServiceMock.Setup(x => x.CreateShortUrlAsync(shortUrl, longUrl, CancellationToken.None)).ReturnsAsync(shortUrl);

        // Act
        var actual = await _handler.Handle(command, CancellationToken.None);

        // Act - This returns the right result why is the _handler which uses this object not returning the result.
        //var value = await _urlServiceMock.Object.CreateShortUrlAsync(shortUrl, longUrl, CancellationToken.None);


        // Assert
        Assert.Equal(shortUrl, actual);
        _urlServiceMock.Verify(x => x.CreateShortUrlAsync(shortUrl, longUrl, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task WhenWithNullShortUrlShouldGenerateShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var command = new CreateShortUrlCommand { LongUrl = longUrl, ShortUrl = null };
        var generatedShortUrl = "generated";

        _validatorMock.Setup(x => x.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
        _urlServiceMock.Setup(x => x.CreateShortUrlAsync(null, longUrl, CancellationToken.None)).ReturnsAsync(generatedShortUrl);

        // Act
        var actual = await _handler.Handle(command, CancellationToken.None);
        // Act - This returns the right result why is the _handler which uses this object not returning the result.
        //var value = await _urlServiceMock.Object.CreateShortUrlAsync(shortUrl, longUrl, CancellationToken.None);

        // Assert
        Assert.Equal(generatedShortUrl, actual);
        _urlServiceMock.Verify(x => x.CreateShortUrlAsync(null, longUrl, CancellationToken.None), Times.Once);
    }
}
