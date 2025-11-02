using loma_api.Dtos;
using loma_api.Extensions;
using loma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace loma_api.Controllers;

[ApiController]
[Route("survey")]
[Authorize]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _service;

    public SurveyController(ISurveyService service)
    {
        _service = service;
    }

    [HttpGet("questions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuestions()
    {
        var questions = await _service.GetQuestionsAsync();
        return Ok(questions);
    }

    [HttpPost("answers")]
    public async Task<IActionResult> SubmitAnswers(SurveyAnswerBatchRequest request)
    {
        await _service.SaveAnswersAsync(request, User.GetUserId());
        return Ok(new { message = "Answers submitted successfully" });
    }
}
