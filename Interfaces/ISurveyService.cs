using loma_api.Dtos;
using loma_api.Models;

namespace loma_api.Services;

public interface ISurveyService
{
    Task<IEnumerable<SurveyQuestion>> GetQuestionsAsync();
    Task SaveAnswersAsync(SurveyAnswerBatchRequest request);
}
