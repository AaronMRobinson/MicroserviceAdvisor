using System.ComponentModel.DataAnnotations;
using static MicroserviceAdvisor.Client.Pages.QuestionsPage;

namespace MicroserviceAdvisor.Client.CustomValidation
{
    public class QuestionAnsweredValidator : ValidationAttribute
    {
        public QuestionAnsweredValidator(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public override bool IsValid(object value)
        {
            QuestionaireModel model = (QuestionaireModel)value;

            if(model.ProjetLifecycleStatus == MicroserviceAdvisor.Shared.Enums.ProjetLifecycle.Unknown)
            {
                return !model.LifeCycleAnswers.Any(o => o.Value < 1);
            }
            else if (model.CurrentProjectArchStatus == MicroserviceAdvisor.Shared.Enums.CurrentProjectArch.Unknown && model.LifeCycleAnswers.Any(o => o.Value >1))
            {
                return !model.CurrentArchAnswers.Any(o => o.Value < 1);
            }
            if (model.MicroserviceSuitabilityStatus == MicroserviceAdvisor.Shared.Enums.MicroserviceSuitability.Unknown)
            {
                return !model.MicroserviceSuitabilityAnswers.Any(o => o.Value < 1);
            }

            return false;
        }
    }
}
