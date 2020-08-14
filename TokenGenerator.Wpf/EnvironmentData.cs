using System.Collections.Generic;

namespace TokenGenerator.Wpf
{

    public class EnvironmentData
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string CryptoKey { get; set; }
        public string SecurityKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int ExpirationInSeconds { get; set; }
        public int NotBeforeInSeconds { get; set; }
        public List<Claims> StringClaims { get; set; }
        public bool? ValidateIssuer { get; set; }
        public bool? ValidateAudience { get; set; }
        public bool? ValidateExpiration { get; set; }
    }
}
