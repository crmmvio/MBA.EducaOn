using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MBA.EducaOn.Security.Data;

public class SecurityDbContext : IdentityDbContext
{
    public bool UsingSqlLite { get; private set; }

    public SecurityDbContext(DbContextOptions<SecurityDbContext> options, IConfiguration configuration) : base(options)
    {
        UsingSqlLite = !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnectionLite"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        if (UsingSqlLite)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));
                foreach (var property in properties)
                {
                    builder.Entity(entityType.Name).Property(property.Name).HasConversion<double>();
                }
            }
        }
    }
}