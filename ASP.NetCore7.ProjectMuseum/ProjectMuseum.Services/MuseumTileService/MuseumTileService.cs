using AutoMapper;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using ProjectMuseum.Repositories.MuseumTileRepository;

namespace ProjectMuseum.Services.MuseumTileService;

public class MuseumTileService : IMuseumTileService
{
    private readonly IMuseumTileRepository _museumTileRepository;
    private readonly IMapper _mapper;

    public MuseumTileService(IMuseumTileRepository museumTileRepository, IMapper mapper)
    {
        _museumTileRepository = museumTileRepository;
        _mapper = mapper;
    }

    public async Task<MuseumTileDto> InsertMuseumTile(MuseumTileDto museumTileDto)
    {
        var museumTile = new MuseumTile
        {
            Id = Guid.NewGuid().ToString(),
            XPosition = museumTileDto.XPosition,
            YPosition = museumTileDto.YPosition,
            Decoration = museumTileDto.Decoration,
            Flooring = museumTileDto.Flooring
        };
        await _museumTileRepository.Insert(museumTile);
        var newMuseumTileDto = _mapper.Map<MuseumTileDto>(museumTile);
        return newMuseumTileDto;
    }

    public async Task<MuseumTileDto?> GetMuseumTileById(string tileId)
    {
        var museumTile = await _museumTileRepository.GetById(tileId);
        var newMuseumTileDto = _mapper.Map<MuseumTileDto>(museumTile);
        return newMuseumTileDto;
    }

    public async Task<List<MuseumTileDto>> GetAllMuseumTiles()
    {
        var museumTiles = await _museumTileRepository.GetAll();
        var museumTileDtos = _mapper.Map<List<MuseumTileDto>>(museumTiles);
        return museumTileDtos;
    }

    public Task<MuseumTile> DeleteMuseumTileById()
    {
        throw new NotImplementedException();
    }
}