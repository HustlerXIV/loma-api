using loma_api.Dtos;
using loma_api.Models;
using loma_api.Repositories;

namespace loma_api.Services;

public class SurveyService : ISurveyService
{
    private readonly SurveyRepository _repo;

    public SurveyService(SurveyRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<SurveyQuestion>> GetQuestionsAsync()
    {
        return await _repo.GetActiveQuestionsAsync();
    }

    public async Task SaveAnswersAsync(SurveyAnswerBatchRequest request, Guid userId)
    {
        foreach (var ans in request.Answers)
        {
            var answer = new SurveyAnswer
            {
                UserId = userId,
                QuestionId = ans.QuestionId,
                Answer = ans.Answer,
                AnsweredAt = DateTime.UtcNow
            };

            await _repo.SaveAnswerAsync(answer);
        }
    }
}
