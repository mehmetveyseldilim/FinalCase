
using System.ComponentModel.DataAnnotations;

namespace Banking.Shared
{
    public class JwtConfiguration
    {
        public string Section { get; set; } = "JwtSettings";
        public string? ValidIssuer { get; set; }
        public string? ValidAudience { get; set; }
        public string? Expires { get; set; }
        [Range(10, 30, ErrorMessage = "liveLoading must be between 10 and 30")]
        public string? liveLoading { get; set; }

        public override string ToString()
        {
            return $"Section: {Section}, ValidIssuer: {ValidIssuer}, Valid Audience: {ValidAudience}, Expires: {Expires}, Live Loading: {liveLoading}";
        }
    }
}