namespace Super.Guacamole.Common;

public static class Configuration
{
    public static readonly string Hostname = Environment.GetEnvironmentVariable("HOSTNAME") ?? "127.0.0.1";
    public static readonly int Port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "3001");
}