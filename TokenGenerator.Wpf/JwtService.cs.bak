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
        public IPrincipal ValidateToken(string token, string validAudience, string validIssuer, string cryptographicKey, 
                                        bool? validateAudience, bool? validateIssuer, bool? validateExpiration)
        {
            //Fetch the Jwt key which comes from key vault
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cryptographicKey));

            var now = DateTime.UtcNow;

            bool lifetimeValidate = validateExpiration == null ? true : (bool)validateExpiration;
            bool issuerValidate = validateIssuer == null ? true : (bool)validateIssuer;
            bool audienceValidate = validateAudience == null ? true : (bool)validateAudience; 

            Microsoft.IdentityModel.Tokens.SecurityToken securityToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudience = validAudience,
                ValidIssuer = validIssuer,
                ValidateLifetime = lifetimeValidate,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = issuerValidate,
                ValidateAudience = audienceValidate,                
                LifetimeValidator = this.LifetimeValidator,
                IssuerSigningKey = securityKey
            };
            //extract and assign the user of the jwt
            Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
            IPrincipal user = Thread.CurrentPrincipal;
            return user;

        }

        public IPrincipal ValidateSecureToken(string token, string validAudience, string validIssuer, string cryptographicKey, string securityKey,
                                        bool? validateAudience, bool? validateIssuer, bool? validateExpiration)
        {
            //Fetch the Jwt key which comes from key vault
            var cryptoKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cryptographicKey));
            var signingKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(securityKey));

            var now = DateTime.UtcNow;

            bool lifetimeValidate = validateExpiration == null ? true : (bool)validateExpiration;
            bool issuerValidate = validateIssuer == null ? true : (bool)validateIssuer;
            bool audienceValidate = validateAudience == null ? true : (bool)validateAudience;

            Microsoft.IdentityModel.Tokens.SecurityToken securityToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudience = validAudience,
                ValidIssuer = validIssuer,
                ValidateLifetime = lifetimeValidate,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = issuerValidate,
                ValidateAudience = audienceValidate,
                LifetimeValidator = this.LifetimeValidator,
                IssuerSigningKey = cryptoKey,
                TokenDecryptionKey = signingKey
            };
            //extract and assign the user of the jwt
            Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
            IPrincipal user = Thread.CurrentPrincipal;
            return user;

        }


        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, Microsoft.IdentityModel.Tokens.SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if(notBefore != null)
            {
                if (DateTime.UtcNow <= notBefore) return false;
            }
            if (expires != null)
            {
                if (DateTime.UtcNow > expires) return false;
            }
            return true;
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
                notBefore: DateTime.UtcNow.AddSeconds(Int32.Parse(notBefore)),
                expires: DateTime.UtcNow.AddSeconds(Int32.Parse(expiresInSeconds)),
                signingCredentials: credentials,
                claims: resultClaims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJSONSecureWebToken(string cryptographicKey, string securityKey, string stringClaims, string validAudience, string validIssuer, string notBefore, string expiresInSeconds)
        {
            var cryptoKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cryptographicKey));
            var credentials = new SigningCredentials(cryptoKey, SecurityAlgorithms.HmacSha256);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new EncryptingCredentials(signingKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
             

            List<Claims> claims = new List<Claims>();
            string value = stringClaims;

            claims = JsonConvert.DeserializeObject<List<Claims>>(value);

            //List<Claim> resultClaims = new List<Claim>();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            foreach (var claim in claims)
            {
                claimsIdentity.AddClaim(new Claim(claim.Type, claim.Value));
            }

            var handler = new JwtSecurityTokenHandler();
             
            var token = handler.CreateJwtSecurityToken(
                issuer: validIssuer,
                audience: validAudience,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow.AddSeconds(Int32.Parse(notBefore)),
                expires: DateTime.UtcNow.AddSeconds(Int32.Parse(expiresInSeconds)),
                issuedAt: DateTime.UtcNow,
                signingCredentials: credentials,
                encryptingCredentials: signingCredentials);

            return handler.WriteToken(token);
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
