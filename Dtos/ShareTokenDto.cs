namespace loma_api.Dtos;

public class CreateShareTokenRequest
{
    public Guid LocationId { get; set; }
}

public class ShareTokenResponse
{
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class RedirectResponse
{
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Link { get; set; }
}
