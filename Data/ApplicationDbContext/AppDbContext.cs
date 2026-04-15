using Data.Interceptors;
using Data.Models;
using Data.Models.AuditAndSoftDeleteModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.ApplicationDbContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }



        private readonly AuditingAndSoftDeleteInterceptor _auditingInterceptor;
        public AppDbContext(DbContextOptions<AppDbContext> options, AuditingAndSoftDeleteInterceptor auditingInterceptor)
            : base(options)
        {
            _auditingInterceptor = auditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditingInterceptor);
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.Username).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.LastLoginAt);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            });




            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1500).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.StockQuantity).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(2500);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            });








            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                var parameter = Expression.Parameter(entityType.ClrType, "e");

                var isDeletedProperty = Expression.Property(
                    parameter,
                    nameof(Base.IsDeleted)
                );

                var filter = Expression.Lambda(
                    Expression.Equal(isDeletedProperty, Expression.Constant(false)),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(filter);
            }





        }
    }
}
