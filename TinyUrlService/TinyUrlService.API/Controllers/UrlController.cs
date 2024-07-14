using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TinyUrlService.Domain.Entities;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Queries;

namespace TinyUrlService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("ReactPolicy")] // Apply CORS policy named "ReactPolicy"
public class UrlController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ActivitySource _activitySource;

    public UrlController(ActivitySource activitySource, IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _activitySource = activitySource ?? throw new ArgumentNullException(nameof(activitySource));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateShortUrlAsync([FromBody] UrlMapping urlMapping, CancellationToken cancellationToken)
    {
        using Activity activity = _activitySource.StartActivity("CreateShortUrl");
        activity?.SetTag("UrlTag", urlMapping.LongUrl);
        var command = new CreateShortUrlCommand { LongUrl = urlMapping.LongUrl, ShortUrl = urlMapping.ShortUrl };
        var shortUrl = await _mediator.Send(command, cancellationToken);
        return Ok(shortUrl);
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {
        using Activity activity = _activitySource.StartActivity("GetLongUrl");
        activity?.SetTag("UrlTag", shortUrl);
        var query = new GetLongUrlQuery { ShortUrl = shortUrl };
        var urlMapping = await _mediator.Send(query, cancellationToken);
        return Ok(urlMapping.LongUrl);
    }

    [HttpDelete("{shortUrl}")]
    public async Task<IActionResult> DeleteShortUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {

        using Activity activity = _activitySource.StartActivity("DeleteShortUrl");
        activity?.SetTag("UrlTag", shortUrl);
        var command = new DeleteShortUrlCommand { ShortUrl = shortUrl };
        var success = await _mediator.Send(command, cancellationToken);
        if (success) return NoContent();
        return NotFound();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        using Activity activity = _activitySource.StartActivity("GetStatistics");
        var query = new GetStatisticsQuery();
        var stats = await _mediator.Send(query, cancellationToken);
        return Ok(stats);
    }
}
