using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHarbor.Domain;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(64)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string SecurityQuestion { get; set; } = string.Empty;

    [Required]
    public string SecurityAnswerHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
