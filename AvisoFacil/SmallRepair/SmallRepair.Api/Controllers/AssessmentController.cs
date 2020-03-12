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
        public readonly AssessmentBusiness _business;

        public AssessmentController(AssessmentBusiness business)
        {
            _business = business;
        }

        #region GET
        [HttpGet("{id}")]
        public ActionResult<Assessment> GetAssessment(int id)
        {
            var assement = _business.Find(new Assessment() { IdAssessment = id, IdCustomer = User.GetClaim("CompanyId") });

            if (assement.Sucess)
            {
                return assement.Object;
            }
            else
            {
                return BadRequest(assement.Error);
            }
        }

        [HttpGet("List", Name = "ListAssessment")]
        public IActionResult ListAssessment(int pageNumber = 1, int pageSize = 10)
        {
            var result = _business
                            .List(new Pagination<Assessment>()
                            {
                                PageNumber = pageNumber,
                                PageSize = pageSize
                            }, new Customer()
                            {
                                IdCustomer = User.GetClaim("CompanyId")
                            });

            if (result.Sucess)
                return Ok(result.Object);
            else
                return BadRequest(result.Error);
        }

        [HttpGet("AssessmentReport/{idAssessment}")]
        public IActionResult GetReport(int idAssessment)
        {
            var report = _business.AssessmentReport(new Assessment() { IdAssessment = idAssessment, IdCustomer = User.GetClaim("CompanyId") });

            var result = HttpContext.User.Claims.FirstOrDefault();

            if (report.Sucess)
                return Ok(report.Object);
            else
                return BadRequest(report.Error);
        }

        #endregion

        #region POST

        [HttpPost("Create")]
        public IActionResult PostAssessment([FromBody] AssessmentViewModel assessmentViewModel)
        {
            Assessment assessment = assessmentViewModel.ToModel();

            assessment.IdCustomer = User.GetClaim("CompanyId");

            var result = _business.Add(assessment);

            if (result.Sucess)
            {
                Assessment assessmentResult = result.Object;
                return Ok(assessmentResult.ToView());
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

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
                    IdCustomer = User.GetClaim("CompanyId")
                }, _part);

                if (result.Sucess)
                    return Ok();
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }

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
                    IdCustomer = User.GetClaim("CompanyId")
                }, manualPart);

                if (result.Sucess)
                    return Ok();
                else
                    return BadRequest(result.Error);
            }
            else
                return BadRequest(ModelState);
        }

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

                var result = _business.AddAdditionalService(new Assessment() { IdAssessment = idAssessment, IdCustomer = User.GetClaim("CompanyId") }, assessmentAdditional);

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
        [HttpDelete("AdditionalService")]
        public IActionResult DeleteAdditionalService(int idAssessment, int IdAssessmentAdditionalService)
        {
            if (IdAssessmentAdditionalService > 0)
            {
                var result = _business
                                .DeleteAdditionalService(new Assessment()
                                {
                                    IdAssessment = idAssessment,
                                    IdCustomer = User.GetClaim("CompanyId")
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
                                    IdCustomer = User.GetClaim("CompanyId")
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
