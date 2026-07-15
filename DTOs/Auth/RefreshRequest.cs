using System.ComponentModel.DataAnnotations;

namespace RoutineFlow.DTOs.Auth;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
