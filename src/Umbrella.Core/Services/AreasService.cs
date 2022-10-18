using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public class AreasService : IAreasService
{
    private readonly IAreasRepository _areasRepository;

    public AreasService(IAreasRepository areasRepository)
    {
        _areasRepository = areasRepository;
    }

    public async Task<IEnumerable<Area>> GetAllAsync()
    {
        return await _areasRepository.GetAllAsync() ?? new List<Area>();
    }    

    public async Task AddAsync(Area area)
    {
        await _areasRepository.AddAsync(area);
    }
}