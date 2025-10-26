namespace loma_api.Dtos;

public class CreateLocationRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PlaceId { get; set; }
    public string? AddressLine { get; set; }
    public string? Link { get; set; }
    public bool IsFavorite { get; set; } = false;
}

public class LocationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PlaceId { get; set; }
    public string? AddressLine { get; set; }
    public string? Link { get; set; }
    public bool IsFavorite { get; set; }
}

public class UpdateLocationRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PlaceId { get; set; }
    public string? AddressLine { get; set; }
    public string? Link { get; set; }
    public bool IsFavorite { get; set; }
}