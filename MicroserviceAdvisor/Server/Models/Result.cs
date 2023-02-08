using MicroserviceAdvisor.Shared.Enums;

namespace MicroserviceAdvisor.Server.Models
{
    public class Result
    {
        public ProjetLifecycle ProjetLifecycle { get; set; }

        public CurrentProjectArch CurrentProjectArch { get; set; }

        public MicroserviceSuitability MicroserviceSuitability{ get; set; }

        public List<string>? ReasonsWhy { get; set; }

        public List<string>? ReasonsWhyNot { get; set; }

        public SuggestionType SuggestionType { get; set; }

        public List<string>? ReasonsForTheSuggestion { get; set; }

        public decimal MicroserviceSuitabilityPercentage { get; set; }
    }
}