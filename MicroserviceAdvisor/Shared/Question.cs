using MicroserviceAdvisor.Shared.Enums;

namespace MicroserviceAdvisor.Shared
{
    public class Question
    {
        public string? Name { get; set; }

        public int Id { get; set; }

        public string? Text { get; set; }

        public QuestionCategory Category { get; set; }

        public List<Answer>? Answers { get; set; }
    }
}