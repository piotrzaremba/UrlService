using FluentValidation;
using MediatR;
using TinyUrlService.Domain.Services.Commands;

namespace TinyUrlService.Domain.Services.Handlers;

public class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IUrlService _urlService;
    private readonly IValidator<CreateShortUrlCommand> _validator;

    public CreateShortUrlHandler(IUrlService urlService, IValidator<CreateShortUrlCommand> validator)
    {
        _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        return await _urlService.CreateShortUrlAsync(request.LongUrl, request.ShortUrl, cancellationToken);
    }
}
 
public class CreateShortUrlValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlValidator()
    {
        RuleFor(request => request.ShortUrl)
            .NotEmpty().WithMessage("URL cannot be empty")
            .When(request => !string.IsNullOrWhiteSpace(request.ShortUrl));

        RuleFor(request => request.LongUrl)
            .NotEmpty().WithMessage("URL cannot be empty")
            .When(request => !string.IsNullOrWhiteSpace(request.LongUrl))
            .Must(IsUrlValid).WithMessage("Invalid URL format")
            .When(request => !string.IsNullOrWhiteSpace(request.LongUrl));
    }

    private bool IsUrlValid(string? url)
    {
        bool result = Uri.TryCreate(url, UriKind.Absolute, result: out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        return result;
    }
}