using Umbrella.Core.Extensions;
using Umbrella.Core.Models;
using Umbrella.Core.Repositories;

namespace Umbrella.Core.Services;

public class DashboardsService : IDashboardsService
{
    private readonly IDashboardsRepository _dashboardRepository;

    public DashboardsService(IDashboardsRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<IEnumerable<Dashboard>> GetAllAsync()
    {
        return await _dashboardRepository.GetAllAsync() ?? new List<Dashboard>();
    }    

    public async Task AddAsync(Dashboard dashboard)
    {
        await _dashboardRepository.AddAsync(dashboard);
    }

    public async Task UpdateAsync(Dashboard dashboard)
    {
        await _dashboardRepository.UpdateAsync(dashboard);
    }

    public async Task DeleteAsync(string id)
    {
        await _dashboardRepository.DeleteAync(id);
    }
}