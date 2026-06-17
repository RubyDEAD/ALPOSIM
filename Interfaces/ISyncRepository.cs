using alposim.DTO;
using alposim.Models;

namespace alposim.Interfaces;

public interface ISyncRepository
{
    Task<IEnumerable<Sync>> GetAll();
    Task<Sync> GetSyncById(Guid id);
    Task<IEnumerable<Sync>> GetSyncByDate(DateTime starDate, DateTime endDate);
    Task<Sync> StartSync();
    Task<Sync> StopSync();
    Task<SyncStatusDto> GetSyncStatus(Guid id);
    Task<Sync> PullSync();

}