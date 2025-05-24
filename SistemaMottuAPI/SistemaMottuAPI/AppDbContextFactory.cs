using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SistemaMottuAPI.Data; 


namespace SistemaMottuAPI;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("OracleConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("A string de conexão 'OracleConnection' não foi encontrada.");
        }

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseOracle(connectionString);

        return new AppDbContext(builder.Options);
    }
}