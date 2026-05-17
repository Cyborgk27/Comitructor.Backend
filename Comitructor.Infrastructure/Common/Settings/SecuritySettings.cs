namespace Comitructor.Infrastructure.Common.Settings
{
    public class SecuritySettings
    {
        public int HashWorkFactor { get; set; } = 11;
        public JwtSettings Jwt { get; set; } = new();
    }
}
