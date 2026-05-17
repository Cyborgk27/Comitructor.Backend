namespace Comitructor.Infrastructure.Common.Settings
{
    public class InfrastructureSettings
    {
        public DatabaseSettings Database { get; set; } = new();
        public SecuritySettings Security { get; set; } = new();
        public CorsSettings Cors { get; set; } = new();
    }
}
