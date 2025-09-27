using System.Data;
using Dapper;
using loma_api.Models;

namespace loma_api.Repositories;

public class SurveyRepository
{
    private readonly IDbConnection _db;

    public SurveyRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<SurveyQuestion>> GetActiveQuestionsAsync()
    {
        var sql = @"SELECT * FROM survey_questions 
                    WHERE is_active = 1 
                    ORDER BY section, display_order";
        return await _db.QueryAsync<SurveyQuestion>(sql);
    }

    public async Task<int> SaveAnswerAsync(SurveyAnswer answer)
    {
        var sql = @"INSERT INTO survey_answers (user_id, question_id, answer, answered_at)
                    VALUES (@UserId, @QuestionId, @Answer, @AnsweredAt)";
        return await _db.ExecuteAsync(sql, answer);
    }
}
