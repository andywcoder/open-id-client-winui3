using System;

namespace Santolibre.OpenIdClient
{
    public class IdentityInformation
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IdentityToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
    }
}
