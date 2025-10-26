namespace loma_api.Models;

public class SurveyQuestion
{
    public int Id { get; set; }
    public string TextQuestion { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? Section { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
