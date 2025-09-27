namespace loma_api.Models;

public class Location
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PlaceId { get; set; }
    public string? AddressLine { get; set; }
    public string? Link { get; set; }
    public bool IsFavorite { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
