using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Witeem.McpServer.McpServer.EntityFrameworkCore;

public class MigrationsDbContextFactory : IDesignTimeDbContextFactory<YourCompanyMcpDbContext>
{
    public YourCompanyMcpDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Path.Combine(basePath, "src", "Witeem.McpServer.HttpApi.Host");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("McpServer");

        var builder = new DbContextOptionsBuilder<YourCompanyMcpDbContext>();
        builder.UseSqlServer(connectionString);

        return new YourCompanyMcpDbContext(builder.Options);
    }
}