using alposim.Models;

namespace alposim.DTO;

public class SyncStatusDto
{
    public Guid SyncId { get; set; }
    public SyncStatus Status { get; set; }
    public DateTime SyncDate { get; set; }    
}