using DataAccess.Models;

namespace DataAccess.ModelDtos.Utility;

// DtoMapper.cs

public static class DtoMapper
{
    public static Game ToEntity(this GameDto dto)
    {
        return new Game
        {
            Id = dto.Id,
            Name = dto.Name,
            CreatedBy = dto.CreatedBy.ToString(),
            // Map other properties as needed
        };
    }

    // Add other mapping methods as needed
}