using MicroserviceAdvisor.Server.Services;
using MicroserviceAdvisor.Shared;
using Microsoft.AspNetCore.Mvc;
using static MicroserviceAdvisor.Client.Pages.QuestionsPage;

namespace MicroserviceAdvisor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly ILogger<QuestionsController> _logger;
        private readonly IQuestionService _questionService;

        public QuestionsController(ILogger<QuestionsController> logger, IQuestionService questionService)
        {
            _logger = logger;
            _questionService = questionService;
        }

        [HttpGet]
        public IEnumerable<Question> Get()
        {
            return _questionService.GetQuestions();
        }

        [HttpPost]
        [Route("Results")]
        public QuestionaireModel Results(QuestionaireModel Model)
        {
            var results = _questionService.GetResults(Model);

            Model.MicroserviceSuitabilityStatus = results.MicroserviceSuitability;
            Model.MicroserviceSuggestion = results.SuggestionType;
            Model.ReasonsWhy = results.ReasonsWhy;
            Model.ReasonsWhyNot = results.ReasonsWhyNot;
            Model.SuitabilityPercentage = results.MicroserviceSuitabilityPercentage;
            return Model;
        }

        [HttpPost]
        [Route("Lifecycle")]
        public QuestionaireModel Lifecycle(QuestionaireModel Model)
        {
            Model.ProjetLifecycleStatus = _questionService.ProjectLifecycleAnalyser(Model);

            return Model;
        }

        [HttpPost]
        [Route("CurrentArchType")]
        public QuestionaireModel CurrentArchType(QuestionaireModel Model)
        {
            Model.CurrentProjectArchStatus = _questionService.CurrentProjectTypeAnalyser(Model);

            return Model;
        }
    }
}