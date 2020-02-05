using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AvisoFacil.Auth.Handler
{
    public class TokenValidationHandler : DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            IEnumerable<string> authzHeaders;
            token = null;
            try
            {
                if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
                    return false;

                var bearerToken = authzHeaders.ElementAt(0);
                token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            }
            catch
            {
                return false;
            }
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;
            try
            {
                if (!TryRetrieveToken(request, out token))
                {
                    statusCode = HttpStatusCode.Unauthorized;
                    return base.SendAsync(request, cancellationToken);
                }

                var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey
                (
                    System.Text.Encoding.Default.GetBytes(ConfigurationManager.AppSettings["Token_SecurityKey"])
                );

                SecurityToken securityToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = ConfigurationManager.AppSettings["Token_Audience"],
                    ValidIssuer = ConfigurationManager.AppSettings["Token_Issuer"],
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null && notBefore != null)
                return notBefore <= DateTime.UtcNow && expires >= DateTime.UtcNow;

            return false;
        }
    }
}
