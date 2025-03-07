using WebSocketBoilerplate;
using System;
using System.Collections.Generic;

namespace DataAccess.ModelDtos
{
    public class GameDto : BaseDto
    {
        public GameDto()
        {
            eventType = "AdminStartsGame"; // Match the handler's expected eventType.
            Players = new List<PlayerDto>();
            Questions = new List<QuestionDto>();
            RoundResults = new List<RoundResultDto>();
        }
        
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PlayerDto> Players { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public List<RoundResultDto> RoundResults { get; set; }
        public Guid CreatedBy { get; set; }
    }
}