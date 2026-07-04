using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace alposim.Models;

public class ProductHistory
{
    [Key]
    public Guid Id { get; set; } 
    public Guid ProductId { get;  set; }
    public string FieldChanged { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

}