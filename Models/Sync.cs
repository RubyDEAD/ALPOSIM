using System.ComponentModel.DataAnnotations;

namespace alposim.Models;

public class Sync
{
    [Key]
    public Guid SyncId { get; set; }
    public DateTime SyncDate { get; set; }
    public SyncStatus Status { get; set; } = SyncStatus.NotSynced;

}



public enum SyncStatus
{
   NotSynced,
   SyncLoading,
   Synced,
}