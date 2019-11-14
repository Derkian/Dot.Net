using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.API.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly TypeCompany _typeCompany = TypeCompany.None;

        public CustomAuthorizeAttribute(TypeCompany typeCompany)
        {
            this._typeCompany = typeCompany;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Company _company = null;

            // Carrega a configuração associada ao Token
            _company = Util.Helper.GetConfiguration();
            
            // Valida se Tipo de Companhia do token é igual ao definido no cabeçalho da Controller
            if (_company.TypeCompany != _typeCompany) 
                actionContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }    
}