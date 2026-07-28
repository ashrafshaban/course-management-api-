namespace CourseManagement.Api.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CourseManagement.Api";
    public string Audience { get; set; } = "CourseManagement.Client";
    public string Key { get; set; } = string.Empty;

    /// <summary>Access token lifetime in minutes. Default 30 per ADR-003 (short expiry, no refresh).</summary>
    public int ExpiryMinutes { get; set; } = 30;
}
