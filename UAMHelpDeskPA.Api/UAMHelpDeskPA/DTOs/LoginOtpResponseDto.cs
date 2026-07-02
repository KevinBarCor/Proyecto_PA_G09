namespace UamHelpDeskPA.Api.DTOs
{
    public record LoginOtpResponseDto
    {
        public string SessionToken { get; init; } = string.Empty;
    }
}