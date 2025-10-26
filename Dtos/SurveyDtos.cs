namespace loma_api.Dtos;

public class SurveyAnswerRequest
{
    public Guid? UserId { get; set; }
    public int QuestionId { get; set; }
    public string? Answer { get; set; }
}

public class SurveyAnswerBatchRequest
{
    public Guid? UserId { get; set; }
    public List<SurveyAnswerRequest> Answers { get; set; } = new();
}
