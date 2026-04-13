using Common.Enums;
using Data.Models.AuditAndSoftDeleteModels;

namespace Data.Models
{
    public class User : Base, IAuditable, ISoftDeletableEntity
    {
        public string? Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public RoleEnum Role { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
