using WebSocketBoilerplate;
using System;
using System.Collections.Generic;

namespace Api.EventHandlers.EventMessageDtos
{
    // Sent when broadcasting a question.
    public class BroadcastQuestionDto : BaseDto
    {
        public BroadcastQuestionDto()
        {
            eventType = "BroadcastQuestion";
            Topic = "lobby"; // default topic if not provided
        }
        public required Guid Id { get; set; }
        public required string QuestionText { get; set; }
        public required List<QuestionOptionDto> QuestionOptions { get; set; } = new();
        public int TimeLimitSeconds { get; set; } = 30;
        public string Topic { get; set; }
    }

    // Represents each option for a question.
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

    // Sent when the time for answering a question is over.
    public class QuestionTimeOverDto : BaseDto
    {
        public QuestionTimeOverDto()
        {
            eventType = "QuestionTimeOver";
        }
        public required Guid QuestionId { get; set; }
        public required string Message { get; set; }
    }

    // Player joins a lobby.
    public class PlayerJoinsLobbyDto : BaseDto
    {
        public PlayerJoinsLobbyDto()
        {
            eventType = "PlayerJoinsLobby";
        }

        public required string PlayerId { get; set; }
        public required string Nickname { get; set; }
        public required string GameId { get; set; }  
        public string Topic => GameId;  
    }


    // Notifies all clients that a new member has joined.
    public class MemberHasJoinedDto : BaseDto
    {
        public MemberHasJoinedDto()
        {
            eventType = "MemberHasJoined";
        }
        public required string MemberId { get; set; }
        public required string Nickname { get; set; }
    }

    // Notifies a client that they have successfully joined.
    public class ServerConfirmsPlayerJoinDto : BaseDto
    {
        public ServerConfirmsPlayerJoinDto()
        {
            eventType = "ServerConfirmsPlayerJoin";
        }
        public required string PlayerId { get; set; }
        public required string Message { get; set; }
    }
    
    // Notifies clients when a member leaves.
    public class MemberHasLeftDto : BaseDto
    {
        public string? MemberId { get; set; }
    }

    // New DTO for player answer submissions.
    public class PlayerSubmitsAnswerDto : BaseDto
    {
        public PlayerSubmitsAnswerDto()
        {
            eventType = "PlayerSubmitsAnswer";
        }
        public required string PlayerId { get; set; }
        public required Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
    }

    // New DTO for game progression events.
    public class GameProgressionDto : BaseDto
    {
        public GameProgressionDto(string message)
        {
            Message = message;
            eventType = "GameProgression";
        }
        public required Guid GameId { get; set; }
        public int CurrentRound { get; set; }
        public int TotalRounds { get; set; }
        public string Message { get; set; }
    }
    
    public class AdminStartsGameDto : BaseDto
    {
        // The BaseDto constructor automatically sets the eventType to "AdminStartsGame" (by stripping "Dto" from the class name).
        public AdminStartsGameDto()
        {
            eventType = "AdminStartsGame";
        }

        // 🆕 Add GameId property
        public required string GameId { get; set; }
    }
    
    // Generic error message.
     public class ServerSendsErrorMessageDto : BaseDto
     {
         public required string Error { get; set; }
     }
}
