using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallRepair.Api.Extensions;
using SmallRepair.Api.Model;
using SmallRepair.Business;
using SmallRepair.Management.Model;

namespace SmallRepair.Api.Controllers
{

    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/v{version:apiVersion}/cliente")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        #region VARIAVEL
        public readonly CompanyBusiness _business;
        #endregion

        #region CONSTRUTOR
        public CompanyController(CompanyBusiness business)
        {
            _business = business;
        }
        #endregion

        #region GET
        /// <summary>
        /// Recupera os dados da Empresa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customer/5
        [HttpGet(Name = "GetCompany")]
        public IActionResult Get()
        {
            var idCompany = User.GetClaim("CompanyId");

            Company customer = _business.Get(idCompany);

            if (customer != null)
            {
                customer.ServiceValues = customer.ServiceValues ?? _business.GetServiceValues(customer.IdCompany);

                return Ok(customer.ToView());
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Lista os serviços adicionais da Empresa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AdditionalService", Name = "GetAdditionalService")]
        public IActionResult GetAdditionalService()
        {
            var idCompany = User.GetClaim("CompanyId");

            Company customer = _business.Get(idCompany);

            if (customer != null)
            {
                customer.AdditionalServices = customer.AdditionalServices ?? _business.GetAdditionalServices(idCompany);

                return Ok(customer?.AdditionalServices?.ToView());
            }
            else
            {
                return NotFound();
            }
        } 
        #endregion
    }
}
