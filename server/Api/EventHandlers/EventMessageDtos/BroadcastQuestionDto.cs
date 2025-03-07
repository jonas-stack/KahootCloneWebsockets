using WebSocketBoilerplate;

namespace Api.EventHandlers.EventMessageDtos
{
    // DTO for broadcasting a question to clients.
    public class BroadcastQuestionDto : BaseDto
    {
        public BroadcastQuestionDto()
        {
            eventType = "BroadcastQuestion";
            Topic = "lobby"; // default topic if not provided
        }

        public required Guid Id { get; set; }
        public required string QuestionText { get; set; }
        public required List<QuestionOptionDto> QuestionOptions { get; set; } = new List<QuestionOptionDto>();
        public int TimeLimitSeconds { get; set; } = 30;
        
        // New property for the topic.
        public string Topic { get; set; }
    }

    // DTO for each option in a question.
    public class QuestionOptionDto : BaseDto
    {
        public QuestionOptionDto()
        {
            eventType = "QuestionOption";
        }
        public required Guid Id { get; set; }
        public required string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }

    // DTO to signal that the time for answering the question is over.
    public class QuestionTimeOverDto : BaseDto
    {
        public QuestionTimeOverDto()
        {
            eventType = "QuestionTimeOver";
        }

        public required Guid QuestionId { get; set; }
        public required string Message { get; set; }
    }
}