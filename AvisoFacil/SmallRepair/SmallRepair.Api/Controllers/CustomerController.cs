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
    public class CustomerController : ControllerBase
    {

        public readonly CustomerBusiness _business;

        public CustomerController(CustomerBusiness business)
        {
            _business = business;
        }

        // GET: api/Customer/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public ClienteViewModel Get(string id)
        {
            Customer customer = _business.GetCustomer(id);

            customer.ServiceValues = customer.ServiceValues ?? _business.GetServiceValues(customer.IdCustomer);

            return customer.ToView();
        }

        [HttpGet("ServicoAdicional/{Id}", Name = "GetAdditionalService")]
        public IList<ServicoAdicionalModel> GetAdditionalService(string id)
        {
            Customer customer = _business.GetCustomer(id);

            customer.AdditionalServices = customer.AdditionalServices ?? _business.GetAdditionalServices(id);

            return customer?.AdditionalServices?.ToView();
        }
    }
}
