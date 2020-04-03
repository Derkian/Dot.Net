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
using SmallRepair.Business.Model;
using SmallRepair.Management.Model;

namespace SmallRepair.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize]
    public class AssessmentController : ControllerBase
    {
        #region VARIAVEL
        public readonly AssessmentBusiness _business;
        #endregion

        #region CONSTRUTOR
        public AssessmentController(AssessmentBusiness business)
        {
            _business = business;
        }
        #endregion

        #region GET
        /// <summary>
        /// Recupera um Assessment
        /// </summary>
        /// <param name="id">Id Assessment</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Assessment> GetAssessment(int id)
        {
            var assement = _business.Find(new Assessment() { IdAssessment = id, IdCompany = User.GetClaim("CompanyId") });

            if (assement.Sucess)
            {
                return assement.Object;
            }
            else
            {
                return BadRequest(assement.Error);
            }
        }

        /// <summary>
        /// Retorna lista de Assessment - Paginado
        /// </summary>        
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost("List", Name = "ListAssessment")]
        public IActionResult ListAssessment([FromBody]Search search,
                                            [FromQuery] int pageNumber = 1,
                                            [FromQuery] int pageSize = 10)
        {
            search = search ?? new Search();

            search.SetCompany(new Company() { IdCompany = User.GetClaim("CompanyId") });

            var result = _business
                            .List(new Pagination<Assessment>()
                            {
                                PageNumber = pageNumber,
                                PageSize = pageSize
                            }, search);

            if (result.Sucess)
                return Ok(result.Object);
            else
                return BadRequest(result.Error);
        }


        /// <summary>
        /// Recupera o sumário do Assessment
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <param name="assessmentVersion"></param>
        /// <returns></returns>
        [HttpGet("Summary")]
        public IActionResult GetReport(int idAssessment, int assessmentVersion)
        {
            var report = _business.GetAssessmentSummary(new Assessment() { IdAssessment = idAssessment, Version = assessmentVersion, IdCompany = User.GetClaim("CompanyId") });

            var result = HttpContext.User.Claims.FirstOrDefault();

            if (report.Sucess)
                return Ok(report.Object);
            else
                return BadRequest(report.Error);
        }

        #endregion

        #region POST

        /// <summary>
        /// Cria um Assessment
        /// </summary>
        /// <param name="assessmentViewModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult PostAssessment([FromBody] AssessmentViewModel assessmentViewModel)
        {
            Assessment assessment = assessmentViewModel.ToModel();

            assessment.IdCompany = User.GetClaim("CompanyId");

            var result = _business.CreateAssessment(assessment);

            if (result.Sucess)
            {
                Assessment assessmentResult = result.Object;
                return Ok(assessmentResult);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        /// <summary>
        /// Conclui o Assessment
        /// </summary>
        /// <param name="assessmentViewModel"></param>
        /// <returns></returns>
        [HttpPut("Complete/{idAssessment}")]
        public IActionResult PutAssessment(int idAssessment)
        {
            Assessment assessment = new Assessment() { IdCompany = User.GetClaim("CompanyId"), IdAssessment = idAssessment };

            var result = _business.UpdateAssessment(assessment, User.GetUser());

            if (result.Sucess)
            {
                Assessment assessmentResult = result.Object;
                return Ok(assessmentResult);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        /// <summary>
        /// Cria uma nova versão para o Assessment
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <returns></returns>
        [HttpPost("CreateVersion/{idAssessment}")]
        public IActionResult AddAssessmentVersion(int idAssessment)
        {
            Assessment assessment = new Assessment() { IdAssessment = idAssessment, IdCompany = User.GetClaim("companyId") };

            var result = _business.CreateVersion(assessment, User.GetUser());

            if (result.Sucess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        /// <summary>
        /// Adiciona uma peça do Baremo
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        [HttpPost("Part/Baremo/{idAssessment}")]
        public IActionResult AddPart(int idAssessment, PartViewModel part)
        {
            if (ModelState.IsValid && idAssessment > 0)
            {
                var _part = new Part()
                {
                    Code = part.Code,
                    Quantity = part.Quantity,
                    MalfunctionType = part.MalfunctionType,
                    IntensityType = part.IntensityType,
                    UnitaryValue = part.UnitaryValue
                };

                var result = _business.AddPart(new Assessment()
                {
                    IdAssessment = idAssessment,
                    IdCompany = User.GetClaim("CompanyId")
                }, _part);

                if (result.Sucess)
                    return Ok();
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }

        /// <summary>
        /// Adiciona uma peça manual
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <param name="manualPartViewModel"></param>
        /// <returns></returns>
        [HttpPost("Part/Manual/{idAssessment}")]
        public IActionResult AddManualPart(int idAssessment, ManualPartViewModel manualPartViewModel)
        {
            if (ModelState.IsValid && idAssessment > 0)
            {
                var manualPart = new Part()
                {
                    Description = manualPartViewModel.Description,
                    Quantity = manualPartViewModel.Quantity,
                    UnitaryValue = manualPartViewModel.UnitaryValue
                };

                var result = _business.AddManualPart(new Assessment()
                {
                    IdAssessment = idAssessment,
                    IdCompany = User.GetClaim("CompanyId")
                }, manualPart);

                if (result.Sucess)
                    return Ok();
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }

        /// <summary>
        /// Incluir um serviço adicional
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <param name="additionalService"></param>
        /// <returns></returns>
        [HttpPost("AdditionalService/{idAssessment}")]
        public IActionResult AddAdditionalService(int idAssessment, AdditionalServiceViewModel additionalService)
        {
            if (ModelState.IsValid && idAssessment > 0)
            {
                var assessmentAdditional = new AdditionalService()
                {
                    Description = additionalService.Description,
                    Value = additionalService.Value
                };

                var result = _business.AddAdditionalService(new Assessment() { IdAssessment = idAssessment, IdCompany = User.GetClaim("CompanyId") }, assessmentAdditional);

                if (result.Sucess)
                    return Ok();
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Remove um serviço adicional
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <param name="IdAssessmentAdditionalService"></param>
        /// <returns></returns>
        [HttpDelete("AdditionalService")]
        public IActionResult DeleteAdditionalService(int idAssessment, int IdAssessmentAdditionalService)
        {
            if (IdAssessmentAdditionalService > 0)
            {
                var result = _business
                                .DeleteAdditionalService(new Assessment()
                                {
                                    IdAssessment = idAssessment,
                                    IdCompany = User.GetClaim("CompanyId")
                                },
                                new AssessmentAdditionalService()
                                {
                                    IdAssessmentAdditionalService = IdAssessmentAdditionalService
                                });

                if (result.Sucess)
                    return Ok(result.Object);
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }

        /// <summary>
        /// Remove uma peça do assessment
        /// </summary>
        /// <param name="idAssessment"></param>
        /// <param name="IdPart"></param>
        /// <returns></returns>
        [HttpDelete("Part/Baremo")]
        public IActionResult DeletePart(int idAssessment, int IdPart)
        {
            if (ModelState.IsValid && idAssessment > 0)
            {
                var _part = new Part()
                {
                    IdPart = IdPart
                };

                var result = _business
                                .DeletePart(new Assessment()
                                {
                                    IdAssessment = idAssessment,
                                    IdCompany = User.GetClaim("CompanyId")
                                }, _part);

                if (result.Sucess)
                    return Ok();
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }
        #endregion
    }
}
