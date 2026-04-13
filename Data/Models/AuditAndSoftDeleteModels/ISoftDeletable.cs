namespace Data.Models.AuditAndSoftDeleteModels
{
    public interface ISoftDeletableEntity : IAuditable
    {
        DateTime? DeletedAt { get; set; }
        string? DeletedBy { get; set; }
    }
}
