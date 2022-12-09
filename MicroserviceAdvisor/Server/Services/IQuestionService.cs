using MicroserviceAdvisor.Server.Models;
using MicroserviceAdvisor.Shared;
using MicroserviceAdvisor.Shared.Enums;
using static MicroserviceAdvisor.Client.Pages.QuestionsPage;

namespace MicroserviceAdvisor.Server.Services
{
    public interface IQuestionService
    {
        public List<Question> GetQuestions();

        public Result GetResults(QuestionaireModel QuestionaireModel);

        public CurrentProjectArch CurrentProjectTypeAnalyser(QuestionaireModel QuestionaireModel);

        public ProjetLifecycle ProjectLifecycleAnalyser(QuestionaireModel QuestionaireModel);

    }
}