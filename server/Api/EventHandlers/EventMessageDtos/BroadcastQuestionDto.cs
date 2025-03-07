using WebSocketBoilerplate;
using System;
using System.Collections.Generic;

namespace Api.EventHandlers.Dtos
{
    // DTO for broadcasting a question to clients.
    public class BroadcastQuestionDto : BaseDto
    {
        public BroadcastQuestionDto()
        {
            eventType = "BroadcastQuestion";
        }

        public required Guid Id { get; set; }
        public required string QuestionText { get; set; }
        public required List<QuestionOptionDto> QuestionOptions { get; set; } = new();
        // Optionally, add additional properties such as a time limit.
        public int TimeLimitSeconds { get; set; } = 30;
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
        public string? RequestId { get; set; }
    }
}