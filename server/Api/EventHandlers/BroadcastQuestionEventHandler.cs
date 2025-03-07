using Api.WebSockets;
using Fleck;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Api.EventHandlers.EventMessageDtos;
using WebSocketBoilerplate;

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
            if (dto == null || string.IsNullOrEmpty(dto.QuestionText) || dto.QuestionOptions == null || dto.QuestionOptions.Count == 0)
            {
                socket.SendDto(new ServerSendsErrorMessageDto
                {
                    Error = "Invalid question data.",
                    requestId = dto?.requestId
                });
                return;
            }

            string topic = string.IsNullOrEmpty(dto.Topic) ? "lobby" : dto.Topic;
            _logger.LogDebug("Broadcasting question {QuestionId} to topic '{Topic}'", dto.Id, topic);
            await _connectionManager.BroadcastToTopic(topic, dto);
            _logger.LogDebug("Question {QuestionId} broadcasted", dto.Id);

            // Wait for the question time limit.
            await Task.Delay(dto.TimeLimitSeconds * 1000);

            var timeOverDto = new QuestionTimeOverDto
            {
                QuestionId = dto.Id,
                Message = "Time is up for answering the question."
            };
            timeOverDto.requestId = dto.requestId;
            await _connectionManager.BroadcastToTopic(topic, timeOverDto);
            _logger.LogDebug("Time-over message broadcasted for question {QuestionId}", dto.Id);
        }
    }
}
