using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallRepair.Api.Extensions;
using SmallRepair.Business;
using SmallRepair.Management.Model;

namespace SmallRepair.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {

        public readonly AssessmentBusiness _business;

        public ReportController(AssessmentBusiness business)
        {
            _business = business;
        }

        // GET: api/Report/5
        [HttpGet("{code}/{idAssessment}", Name = "Get")]
        public async Task<IActionResult> Get(string code, int idAssessment)
        {
            var assement = await _business.ReportPDFAsync(new Assessment()
            {
                IdAssessment = idAssessment,
                IdCompany = User.GetClaim("CompanyId")
            },
                            User.GetClaim("CompanyId"),
                            code);

            if (assement.Sucess)
            {   
                return new FileStreamResult(assement.Object, "application/pdf");
            }
            else
            {
                return BadRequest(assement.Error);
            }
        }
    }
}
