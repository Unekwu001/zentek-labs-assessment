using Data.Models;

namespace Core.Repos.UserRepositories
{
    public interface IUserRepo
    {
        Task<User> AddAsync(User user);
        Task AddRangeAsync(IEnumerable<User> users);

        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUserNameAsync(string userName);

        Task<bool> ExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email);

        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetActiveUsersAsync();

        Task<IEnumerable<User>> SearchAsync(string searchTerm);

        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            bool? isActive = null);
    }
}
