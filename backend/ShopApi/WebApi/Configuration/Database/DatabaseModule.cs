using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Configuration.Database;

public static class DatabaseModule
{
    public static void MigrateAndSeed(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<ShopDbContext>();

            if (dbContext == null) throw new NullReferenceException();
            
            if(!app.Environment.IsDevelopment())
                dbContext.Database.Migrate();
            
            dbContext.SeedProduct();
        }
    }
}