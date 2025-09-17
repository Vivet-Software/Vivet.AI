using System;
using Npgsql;
using Vivet.AI.Config;

namespace Vivet.AI.Extensions.Embeddings.Postgres.Extensions;

internal static class VectorStoreOptionsExtensions
{
    private const string DATABASE_NAME = "vectors";

    internal static string BuildConnectionString(this VectorStoreOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        var host = options.Host;
        var port = options.Port;

        if (options.Host.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || options.Host.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(options.Host);

            host = uri.Host;
            port = uri.Port > 0 ? uri.Port : port;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Username = options.Username,
            Password = options.ApiKey,
            Database = DATABASE_NAME,
            Timeout = (int)options.Timeout.TotalSeconds,
            CommandTimeout = (int)options.Timeout.TotalSeconds,
            SslMode = options.UseSsl 
                ? SslMode.Require 
                : SslMode.Disable
        };

        return builder.ConnectionString;
    }
}