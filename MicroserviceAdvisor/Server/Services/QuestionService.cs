using MicroserviceAdvisor.Server.Models;
using MicroserviceAdvisor.Shared;
using MicroserviceAdvisor.Shared.Enums;
using Newtonsoft.Json;
using static MicroserviceAdvisor.Client.Pages.QuestionsPage;

namespace MicroserviceAdvisor.Server.Services
{
    public class QuestionService : IQuestionService
    {
        public List<Question> GetQuestions()
        {
            using StreamReader streamReader = new(Environment.CurrentDirectory+"//Questions//Questions.json");
            string questionsFile = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Question>>(questionsFile);
        }

        public Result GetResults(QuestionaireModel QuestionaireModel)
        {
            var questions = GetQuestions();
            var reasonsForMicroservice = new List<string>();
            var reasonsAgainstMicroservice = new List<string>();

            QuestionaireModel.MicroserviceSuitabilityStatus = SuitabilityAnalyser(QuestionaireModel, questions, out reasonsForMicroservice, out reasonsAgainstMicroservice);
            
            var microserviceSuitabilityPercentage = Convert.ToDecimal(reasonsForMicroservice.Count) / Convert.ToDecimal(reasonsForMicroservice.Count + reasonsAgainstMicroservice.Count)*100;

            return new Result
            {
                CurrentProjectArch = QuestionaireModel.CurrentProjectArchStatus,
                ProjetLifecycle = QuestionaireModel.ProjetLifecycleStatus,
                MicroserviceSuitability = QuestionaireModel.MicroserviceSuitabilityStatus,
                ReasonsWhy = reasonsForMicroservice,
                ReasonsWhyNot = reasonsAgainstMicroservice,
                SuggestionType = SuggestionAnalyser(QuestionaireModel),
                ReasonsForTheSuggestion = new List<string> { "", "" },
                MicroserviceSuitabilityPercentage = microserviceSuitabilityPercentage
            };
        }

        public CurrentProjectArch CurrentProjectTypeAnalyser(QuestionaireModel QuestionaireModel)
        {
            var questions = GetQuestions();
            var monolithTally = 0;
            var microserviceTally = 0;

            var archType = CurrentProjectArch.Unknown;
            foreach (var currentArchAnswer in QuestionaireModel.CurrentArchAnswers)
            {
                var question = questions.SingleOrDefault(o => o.Id == currentArchAnswer.QuestionId);
                var answer = question.Answers.SingleOrDefault(o => o.Value == currentArchAnswer.Value);

                if (question.Category == QuestionCategory.CurrentArchType)
                {
                    Enum.TryParse(answer.Meaning, out archType);

                    if (archType == CurrentProjectArch.Microservice)
                    {
                        microserviceTally++;
                    }
                    else if (archType == CurrentProjectArch.Monolith)
                    {
                        monolithTally++;
                    }
                }
            }

            if (microserviceTally > monolithTally)
            {
                return CurrentProjectArch.Microservice;
            }
            else
            {
                return CurrentProjectArch.Monolith;
            }
        }

        public ProjetLifecycle ProjectLifecycleAnalyser(QuestionaireModel QuestionaireModel)
        {
            var questions = GetQuestions();
            var projetLifecycle = ProjetLifecycle.Unknown;
            foreach (var currentArchAnswer in QuestionaireModel.LifeCycleAnswers)
            {
                var question = questions.SingleOrDefault(o => o.Id == currentArchAnswer.QuestionId);
                var answer = question.Answers.SingleOrDefault(o => o.Value == currentArchAnswer.Value);

                if (question.Category == QuestionCategory.Lifecycle)
                {
                    Enum.TryParse(answer.Meaning, out projetLifecycle);
                }
            }
            return projetLifecycle;
        }

        private static MicroserviceSuitability SuitabilityAnalyser(QuestionaireModel QuestionaireModel, List<Question>? questions, out List<string>reasonsForMicroservice,out List<string>reasonsAgainstMicroservice)
        {
            var suitableTally = 0;
            var notSuitableTally = 0;
            var microserviceSuitability = MicroserviceSuitability.Unknown;
            reasonsForMicroservice = new List<string>();
            reasonsAgainstMicroservice  = new List<string>();

            foreach (var currentArchAnswer in QuestionaireModel.MicroserviceSuitabilityAnswers)
            {
                var question = questions.SingleOrDefault(o => o.Id == currentArchAnswer.QuestionId);
                var answer = question.Answers.SingleOrDefault(o => o.Value == currentArchAnswer.Value);

                if (question.Category == QuestionCategory.MicroserviceSuitability)
                {
                    Enum.TryParse(answer.Meaning, out microserviceSuitability);

                    if (microserviceSuitability == MicroserviceSuitability.Suitable)
                    {
                        suitableTally++;

                        reasonsForMicroservice.Add(answer.Conclusion);
                    }
                    else if (microserviceSuitability == MicroserviceSuitability.NotSuitable)
                    {
                        notSuitableTally++;
                        reasonsAgainstMicroservice.Add(answer.Conclusion);
                    }
                }
            }

            if (suitableTally > notSuitableTally)
            {
                return MicroserviceSuitability.Suitable;
            }
            else
            {
                return MicroserviceSuitability.NotSuitable;
            }
        }


        public static SuggestionType SuggestionAnalyser(QuestionaireModel questionaireModel)
        {
            if(questionaireModel.ProjetLifecycleStatus == ProjetLifecycle.New)
            {
                if(questionaireModel.MicroserviceSuitabilityStatus == MicroserviceSuitability.NotSuitable)
                {
                    return SuggestionType.BuildAMonolith;
                }
                else if(questionaireModel.MicroserviceSuitabilityStatus == MicroserviceSuitability.Suitable)
                {
                    return SuggestionType.BuildAMicroservice;
                }
            }
            else if(questionaireModel.ProjetLifecycleStatus == ProjetLifecycle.Existing)
            {
                if (questionaireModel.CurrentProjectArchStatus == CurrentProjectArch.Microservice)
                {
                    if (questionaireModel.MicroserviceSuitabilityStatus == MicroserviceSuitability.NotSuitable)
                    {
                        return SuggestionType.MergeMicroservice;
                    }
                    else if (questionaireModel.MicroserviceSuitabilityStatus == MicroserviceSuitability.Suitable)
                    {
                        return SuggestionType.PotentialMicroservicesImprovements;
                    }
                }
                else if (questionaireModel.CurrentProjectArchStatus == CurrentProjectArch.Monolith)
                {
                    if (questionaireModel.MicroserviceSuitabilityStatus == MicroserviceSuitability.NotSuitable)
                    {
                        return SuggestionType.StayAMonolith;
                    }
                    else if (questionaireModel.MicroserviceSuitabilityStatus == MicroserviceSuitability.Suitable)
                    {
                        return SuggestionType.TransformIntoAMicroservice;
                    }
                }
            }
            return SuggestionType.Unknown;
        }
    }
}
