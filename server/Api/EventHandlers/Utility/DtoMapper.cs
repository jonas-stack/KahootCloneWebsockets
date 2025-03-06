namespace Api.EventHandlers.Utility;

// DtoMapper.cs
using DataAccess.Models;
using Api.EventHandlers.Dtos;

public static class DtoMapper
{
    public static Game ToEntity(this GameDto dto)
    {
        return new Game
        {
            Id = dto.Id,
            Name = dto.Name,
            // Map other properties as needed
        };
    }

    // Add other mapping methods as needed
}