using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Witeem.McpServer.EntityFrameworkCore;

/// <summary>
/// Design-time factory for <see cref="WiteemMcpServerDbContext"/> used by
/// <c>dotnet ef</c>. Falls back to the HttpApi.Host appsettings when run from
/// the solution root. Resolves the unified "Default" connection string.
/// </summary>
public class WiteemMcpServerMigrationsDbContextFactory : IDesignTimeDbContextFactory<WiteemMcpServerDbContext>
{
    public WiteemMcpServerDbContext CreateDbContext(string[] args)
    {
        var basePath = ResolveConfigurationDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Default");

        var builder = new DbContextOptionsBuilder<WiteemMcpServerDbContext>();
        builder.UseSqlServer(connectionString);

        return new WiteemMcpServerDbContext(builder.Options);
    }

    /// <summary>
    /// Walk up from the design-time working directory (usually the EF Core
    /// project directory) until the host appsettings.json is found. Works both
    /// when invoked from the solution root and from an individual project.
    /// </summary>
    private static string ResolveConfigurationDirectory()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory != null)
        {
            var hostPath = Path.Combine(directory.FullName, "src", "Witeem.McpServer.HttpApi.Host");
            if (File.Exists(Path.Combine(hostPath, "appsettings.json")))
            {
                return hostPath;
            }

            if (File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(
            "Unable to locate appsettings.json for design-time migrations. " +
            "Run dotnet ef from the solution directory or the HttpApi.Host project directory.");
    }
}
