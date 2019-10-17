using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Models
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        [Unity.Dependency]
        public ConfigurationService _configurationService { get; set; }
       
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string realm = actionContext.Request.RequestUri.Host;

            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

                if (actionContext.Response.StatusCode == HttpStatusCode.Unauthorized)
                    actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", realm)); 
            }
            else
            {
                if (actionContext.Request.Headers.Authorization.Parameter != null)
                {
                    string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                    string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                    string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                    string login = usernamePasswordArray[0];
                    string password = usernamePasswordArray[1];

                    // Busca dados da Configuração pela Companhia informada na autenticação
                    Configuration _configuration = _configurationService.GetAuthenticatedCompany(login, password);

                    // Autentica os dados da Companhia informada no header do request
                    if (_configuration != null)
                    {
                        var identity = new GenericIdentity(login);
                        identity.AddClaim(new Claim(ClaimTypes.Sid, _configuration.Id.ToString()));
                        identity.AddClaim(new Claim(ClaimTypes.Name, _configuration.Name.ToString()));
                        IPrincipal principal = new GenericPrincipal(identity, null);
                        Thread.CurrentPrincipal = principal;

                        if (HttpContext.Current != null)
                            HttpContext.Current.User = principal;
                    }
                    else
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}