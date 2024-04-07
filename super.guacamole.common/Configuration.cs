namespace super.guacamole.common;

public class Configuration
{
    public string Hostname { get; private set; } = Environment.GetEnvironmentVariable("HOSTNAME") ?? "127.0.0.1";
    public int Port { get; private set; } = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "3001");
}