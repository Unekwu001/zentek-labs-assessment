
using System.ComponentModel.DataAnnotations;

namespace Common.Dtos; 
public class RegisterUserDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
    [RegularExpression(
        @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must contain at least one letter and one number"
    )]
    public string Password { get; set; } = default!;
}
