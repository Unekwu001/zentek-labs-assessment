using Data.ApplicationDbContext;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repos.ProductRepositories
{

    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }




        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }






        public async Task AddRangeAsync(IEnumerable<Product> products)
        {
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }





        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }





        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }





        public async Task DeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }





        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }





        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }






        public async Task<IEnumerable<Product>> GetByColourAsync(string colour)
        {
            return await _context.Products
                .AsNoTracking().Where(p => p.Colour.ToLower() == colour.Trim().ToLower())
                .ToListAsync();
        }





        public async Task<IEnumerable<Product>> GetByMultipleColoursAsync(IEnumerable<string> colours)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => colours.Contains(p.Colour))
                .ToListAsync();
        }





        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }




        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();

            return await _context.Products
                .AsNoTracking()
                .Where(p => p.Name.ToLower().Contains(searchTerm) ||
                            (p.Description != null && p.Description.ToLower().Contains(searchTerm)))
                .ToListAsync();
        }





        public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold = 10)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.StockQuantity < threshold) 
                .ToListAsync();
        }





        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? colour = null,
            decimal? minPrice = null,
            string? sortBy = null,
            bool ascending = true)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(colour))
            {
               query = query.Where(p => p.Colour == colour.ToLower());
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                "colour" => ascending ? query.OrderBy(p => p.Colour) : query.OrderByDescending(p => p.Colour),
                _ => query.OrderBy(p => p.Name)   // default
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }





}