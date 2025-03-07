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

            // Use the topic from the DTO, defaulting to "lobby" if not specified.
            string topic = string.IsNullOrEmpty(dto.Topic) ? "lobby" : dto.Topic;
            _logger.LogDebug("Broadcasting question {QuestionId} to topic '{Topic}'", dto.Id, topic);

            // Broadcast the question to all clients in the specified topic.
            await _connectionManager.BroadcastToTopic(topic, dto);
            _logger.LogDebug("Broadcasted question {QuestionId} to topic '{Topic}'", dto.Id, topic);

            // Start the timer for this question.
            _logger.LogDebug("Starting {TimeLimitSeconds}-second timer for question {QuestionId}", dto.TimeLimitSeconds, dto.Id);
            await Task.Delay(dto.TimeLimitSeconds * 1000);

            // After the timer, broadcast a message indicating that time is up.
            var timeOverDto = new QuestionTimeOverDto
            {
                QuestionId = dto.Id,
                Message = "Time is up for answering the question."
            };
            timeOverDto.requestId = dto.requestId;

            await _connectionManager.BroadcastToTopic(topic, timeOverDto);
            _logger.LogDebug("Broadcasted end-of-question message for question {QuestionId} in topic '{Topic}'", dto.Id, topic);
        }
    }
}
