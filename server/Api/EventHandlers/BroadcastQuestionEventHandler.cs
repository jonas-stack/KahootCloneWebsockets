using Api.EventHandlers.Dtos;
using Api.WebSockets;
using Fleck;
using WebSocketBoilerplate;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api.EventHandlers
{
    public class BroadcastQuestionEventHandler : BaseEventHandler<BroadcastQuestionDto>
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<BroadcastQuestionEventHandler> _logger;

        public BroadcastQuestionEventHandler(IConnectionManager connectionManager, ILogger<BroadcastQuestionEventHandler> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task Handle(BroadcastQuestionDto dto, IWebSocketConnection socket)
        {
            // Validate the incoming DTO.
            if (dto == null || string.IsNullOrEmpty(dto.QuestionText) || dto.QuestionOptions == null || dto.QuestionOptions.Count == 0)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid question data.",
                    requestId = dto?.requestId
                });
                return;
            }

            _logger.LogDebug("BroadcastQuestionEventHandler invoked for question: {QuestionId}", dto.Id);

            // Choose the topic to broadcast to. Here we use "game" as an example.
            string topic = "game";

            // Broadcast the question to all clients subscribed to the topic.
            await _connectionManager.BroadcastToTopic(topic, dto);
            _logger.LogDebug("Broadcasted question {QuestionId} to topic '{Topic}'", dto.Id, topic);

            // Start a 30-second timer (or use dto.TimeLimitSeconds if you want it dynamic).
            _logger.LogDebug("Starting 30-second timer for question {QuestionId}", dto.Id);
            await Task.Delay(dto.TimeLimitSeconds * 1000);

            // After the timer ends, broadcast a message indicating that time is up.
            var timeOverDto = new QuestionTimeOverDto
            {
                QuestionId = dto.Id,
                Message = "Time is up for answering the question.",
                RequestId = dto.requestId
            };

            await _connectionManager.BroadcastToTopic(topic, timeOverDto);
            _logger.LogDebug("Broadcasted end-of-question message for question {QuestionId}", dto.Id);
        }
    }
}
