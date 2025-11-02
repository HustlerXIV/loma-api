namespace loma_api.Dtos;

public class SurveyAnswerRequest
{
    public int QuestionId { get; set; }
    public string? Answer { get; set; }
}

public class SurveyAnswerBatchRequest
{
    public List<SurveyAnswerRequest> Answers { get; set; } = new();
}
