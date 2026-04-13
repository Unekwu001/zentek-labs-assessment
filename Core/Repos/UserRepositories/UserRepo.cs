using Data.AppDbContext;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repos.UserRepositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }




        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }




        public async Task AddRangeAsync(IEnumerable<User> users)
        {
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }




        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }




        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }




        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userName);
        }





        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == id);
        }





        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }





        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }




        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }





        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }





        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .ToListAsync();
        }





        public async Task<IEnumerable<User>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();

            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.Username.ToLower().Contains(searchTerm) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)))
                .ToListAsync();
        }





        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            bool? isActive = null)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(search) ||
                    u.Username.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Username)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }




    }

}
