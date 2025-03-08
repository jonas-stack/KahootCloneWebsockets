using WebSocketBoilerplate;
using System;
using System.Collections.Generic;
using Api.WebSockets;
using DataAccess.ModelDtos;

namespace Api.EventHandlers.EventMessageDtos
{
    public class AdminStartsGameDto : CustomBaseDto
    {
        public AdminStartsGameDto() => eventType = "AdminStartsGame";
        public required string GameId { get; set; }
    }

    public class AdminStartsNextRoundDto : CustomBaseDto
    {
        public AdminStartsNextRoundDto() => eventType = "AdminStartsNextRound";
        public required string GameId { get; set; }
        public int RoundNumber { get; set; }
    }
    
    public class GameStartedDto : CustomBaseDto
    {
        public GameStartedDto() => eventType = "GameStarted";
        public string GameId { get; set; } = string.Empty;
        public string Message { get; set; } = "The game has started!";
    }

    public class RoundStartedDto : CustomBaseDto
    {
        public RoundStartedDto() => eventType = "RoundStarted";
        public int RoundNumber { get; set; }
        public required QuestionDto Question { get; set; } // ✅ Now uses the correct DTO from DataAccess.ModelDtos
    }
    
    public class PlayerSubmitsAnswerDto : CustomBaseDto
    {
        public PlayerSubmitsAnswerDto()
        {
            eventType = "PlayerSubmitsAnswer";
        }

        public required string PlayerId { get; set; } // The ID of the player submitting the answer
        public required Guid QuestionId { get; set; } // The question being answered
        public Guid? SelectedOptionId { get; set; } // The selected answer option (nullable if no selection)
    }
    
    public class ServerConfirmsPlayerJoinDto : CustomBaseDto
    {
        public ServerConfirmsPlayerJoinDto()
        {
            eventType = "ServerConfirmsPlayerJoin";
        }

        public required string PlayerId { get; set; } // The ID of the player that joined
        public required string Message { get; set; } // Confirmation message
    }
    
    public class GameProgressionDto : CustomBaseDto
    {
        public GameProgressionDto()
        {
            eventType = "GameProgression";
        }

        public required Guid GameId { get; set; } // The game being progressed
        public int CurrentRound { get; set; } // The current round number
        public int TotalRounds { get; set; } // Total number of rounds in the game
        public required string Message { get; set; } // Status message (e.g., "Round 2 has started", "Game Over")
    }
    
    public class MemberHasJoinedDto : CustomBaseDto
    {
        public MemberHasJoinedDto()
        {
            eventType = "MemberHasJoined";
        }

        public required string MemberId { get; set; } // The ID of the player who joined
        public required string Nickname { get; set; } // The nickname of the player
    }
    
    public class PlayerJoinsLobbyDto : CustomBaseDto
    {
        public PlayerJoinsLobbyDto()
        {
            eventType = "PlayerJoinsLobby";
        }

        public required string PlayerId { get; set; } // Unique identifier for the player
        public required string Nickname { get; set; } // Player's chosen name
        public required string GameId { get; set; }  // The game lobby the player is joining
        public string Topic => GameId;  // GameId is used as the WebSocket topic
    }
    
    public class MemberHasLeftDto : CustomBaseDto
    {
        public MemberHasLeftDto()
        {
            eventType = "MemberHasLeft";
        }

        public required string MemberId { get; set; } 
        public string? GameId { get; set; }
    }

    public class ServerSendsErrorMessageDto : CustomBaseDto
    {
        public ServerSendsErrorMessageDto() => eventType = "ServerSendsErrorMessage";
        public required string Error { get; set; }
    }
}