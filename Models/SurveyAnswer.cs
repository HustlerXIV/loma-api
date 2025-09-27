namespace loma_api.Models;

public class SurveyAnswer
{
    public int Id { get; set; }
    public Guid? UserId { get; set; }
    public int QuestionId { get; set; }
    public string? Answer { get; set; }
    public DateTime AnsweredAt { get; set; }
}
