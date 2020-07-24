using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TokenGenerator.Wpf
{

    public class JwtService
    {
        public IPrincipal ValidateToken(string token, string validAudience, string validIssuer, string cryptographicKey)
        {
            //Fetch the Jwt key which comes from key vault
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cryptographicKey));

            var now = DateTime.UtcNow;

            Microsoft.IdentityModel.Tokens.SecurityToken securityToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudience = validAudience,
                ValidIssuer = validIssuer,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                LifetimeValidator = this.LifetimeValidator,
                IssuerSigningKey = securityKey
            };
            //extract and assign the user of the jwt
            Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
            IPrincipal user = Thread.CurrentPrincipal;
            return user;

        }
        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, Microsoft.IdentityModel.Tokens.SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
        public string GenerateJSONWebToken(string cryptographicKey, string stringClaims, string validAudience, string validIssuer, string notBefore, string expiresInSeconds)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cryptographicKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claims> claims = new List<Claims>();
            string value = stringClaims;

            claims = JsonConvert.DeserializeObject<List<Claims>>(value);

            List<Claim> resultClaims = new List<Claim>();

            foreach (var claim in claims)
            {
                resultClaims.Add(new Claim(claim.Type, claim.Value));
            }

            var token = new JwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                notBefore: DateTime.Now.AddSeconds(Int32.Parse(notBefore)),
                expires: DateTime.Now.AddSeconds(Int32.Parse(expiresInSeconds)),
                signingCredentials: credentials,
                claims: resultClaims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetClaims(ClaimsPrincipal user)
        {
            string result = "";
            if (null != user)
            {
                foreach (Claim claim in user.Claims)
                {
                    result += "CLAIM TYPE: " + claim.Type + "; CLAIM VALUE: " + claim.Value + '\r';
                }
            }
            return result;
        }
    }
}
