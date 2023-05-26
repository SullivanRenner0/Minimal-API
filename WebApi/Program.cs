using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
        builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);
        var app = builder.Build();

        app.MapGet("/", () => "Minimal API von Sullivan");

        _ = app.MapGet("/check", (IOptions<DatabaseSettings> options) =>
                {
                    string connectionString = "mongodb://mongodb:27017";

                    // Check connection without credentials
                    bool hasAccessWithoutLogin = CheckMongoDBConnection(connectionString, out var databases, out _);
                    if (hasAccessWithoutLogin)
                    {
                        return "You have access to MongoDB without login credentials." + Environment.NewLine +
                        "DBS: " + String.Join(", ", databases);
                    }
                    else
                    {

                        string authenticatedConnectionString = options.Value.ConnectionString;

                        // Check connection with login credentials
                        bool hasAccessWithLogin = CheckMongoDBConnection(authenticatedConnectionString, out databases, out var error);
                        if (hasAccessWithLogin)
                        {
                            return "You have access to MongoDB with login credentials." + Environment.NewLine +
                            "DBS: " + String.Join(", ", databases);
                        }
                        else
                        {
                            return $"Failed to connect to MongoDB with login credentials." + Environment.NewLine + error;
                        }
                    }
                });

        static bool CheckMongoDBConnection(string conn, out List<string> databases, [NotNullWhen(false)] out string? error)
        {
            error = null;
            databases = new List<string>();
            try
            {
                var mongoClient = new MongoClient(conn);
                databases = mongoClient.ListDatabaseNames().ToList();
                return databases != null && databases.Count > 0;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }

        app.Run();
    }
}