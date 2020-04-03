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
        #region VARIAVEL
        public readonly AssessmentBusiness _business;
        #endregion

        #region CONSTRUTOR
        public ReportController(AssessmentBusiness business)
        {
            _business = business;
        }
        #endregion

        #region GET
        /// <summary>
        /// Executa um relatório
        /// </summary>
        /// <param name="code">Código do Relatório</param>
        /// <param name="idAssessment">Id Assessment</param>
        /// <param name="assessmentVersion">Versão</param>
        /// <returns>PDF</returns>
        // GET: api/Report/5
        [HttpGet("{code}", Name = "Get")]
        public async Task<IActionResult> Get(string code, int idAssessment, int assessmentVersion)
        {
            var assement = await _business.ReportPDFAsync(new Assessment()
            {
                IdAssessment = idAssessment,
                IdCompany = User.GetClaim("CompanyId"),
                Version = assessmentVersion
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

        /// <summary>
        /// Executa um relatório
        /// </summary>
        /// <param name="code">Código do Relatório</param>
        /// <param name="idAssessment">Id Assessment</param>
        /// <param name="assessmentVersion">Versão</param>
        /// <returns>HTML</returns>
        [HttpGet("HTML/{code}", Name = "GetHTML")]
        public async Task<IActionResult> GetHMLT(string code, int idAssessment, int assessmentVersion)
        {
            var assement = await _business.ReportHTMLAsync(new Assessment()
            {
                IdAssessment = idAssessment,
                IdCompany = User.GetClaim("CompanyId"),
                Version = assessmentVersion

            },
                                                            User.GetClaim("CompanyId"),
                                                            code);

            if (assement.Sucess)
            {
                return Ok(assement.Object);
            }
            else
            {
                return BadRequest(assement.Error);
            }
        }
        #endregion

        #region POST
        /// <summary>
        /// Envia por e-mail o relatório do orçamento
        /// </summary>
        /// <param name="code">Código do Relatório</param>
        /// <param name="idAssessment">Id Assessment</param>
        /// <param name="assessmentVersion"></param>
        /// <param name="To"></param>
        /// <returns>Boolean</returns>
        [HttpPost("SendEmail/{code}", Name = "SendEmail")]
        public async Task<IActionResult> SendEmail(string code, int idAssessment, int assessmentVersion, [FromBody] List<string> To)
        {
            var assement = await _business.SendReportByEmail(To, new Assessment()
            {
                IdAssessment = idAssessment,
                Version = assessmentVersion,
                IdCompany = User.GetClaim("CompanyId")
            },
            code);

            if (assement.Sucess)
            {
                return Ok(assement.Object);
            }
            else
            {
                return BadRequest(assement.Error);
            }
        } 
        #endregion
    }
}
