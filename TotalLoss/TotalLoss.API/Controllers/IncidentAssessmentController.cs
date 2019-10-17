using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using TotalLoss.API.Models;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class IncidentAssessmentController : ApiController
    {
        #region | Objects

        private IncidentAssessmentService _incidentAssessmentService;

        #endregion

        #region | Constructor 

        public IncidentAssessmentController(IncidentAssessmentService incidentAssessmentService)
        {
            this._incidentAssessmentService = incidentAssessmentService;
        }

        #endregion

        #region | Actions 

        [HttpPost]
        [Route("api/IncidentsAssessment/Answer/{key}")]
        public async Task<IHttpActionResult> AddIncidentAssessmentAnswer([FromUri, Required(AllowEmptyStrings = false)] string key, 
                                                                         [FromBody] List<Question> questions)
        {
            IncidentAssessment _incidentAssessment;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                else if (questions == null)
                    return BadRequest("");

                // Busca o Incidente pela chave informada
                _incidentAssessment = this._incidentAssessmentService.FindByKey(key);

                // Verifica se o Incidente informado está cadastrado
                if (_incidentAssessment == null)
                    return NotFound();
                
                _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.AddAnswers(_incidentAssessment, questions));

                return Ok(_incidentAssessment);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("api/IncidentsAssessment/Image/{key}")]
        public async Task<IHttpActionResult> AddIncidentAssessmentImage([FromUri, Required(AllowEmptyStrings = false)] string key, [FromBody] SendImage image)
        {
            byte[] ImageBinaryContent;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                else if (image == null)
                    return BadRequest("");


                if (image.File.InputStream.CanSeek)
                    image.File.InputStream.Seek(0, SeekOrigin.Begin);

                using (var br = new BinaryReader(image.File.InputStream))
                {
                    ImageBinaryContent = br.ReadBytes(image.File.ContentLength);
                }

                IncidentAssessmentImage incidentImage = new IncidentAssessmentImage()
                {
                    MimeType = image.File.ContentType,
                    Name = image.File.FileName,
                    //Size = image.File.ContentLength,
                    Image = ImageBinaryContent
                };

                IncidentAssessment _incidentAssessment = this._incidentAssessmentService.FindByKey(key);

                bool result = await Task.Run(() => this._incidentAssessmentService.AddImage(_incidentAssessment, incidentImage));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/IncidentsAssessment/GetIncident/{key}")]
        public async Task<IHttpActionResult> GetIncidentAssessmentByKey([FromUri, Required(AllowEmptyStrings = false)] string key)
        {
            IncidentAssessment _incidentAssessment = null;
            try
            {
                // Busca Incidente por ID criptografado
                _incidentAssessment = await Task.Run(() => _incidentAssessmentService.FindByKey(key));

                // Verifica se existe Incidente por ID informado
                if (_incidentAssessment == null)
                    return NotFound();
            }
            catch (System.Exception ex)
            {
                // Retorna Json de Erro interno gerado 
                return InternalServerError(new System.Exception(ex.Message));
            }

            // Retorna response com todas Categorias por Campanhia
            return Ok(_incidentAssessment);
        }

        [HttpPost]
        [Route("api/IncidentsAssessment/Create")]
        [BasicAuthentication]
        public async Task<IHttpActionResult> Post([FromBody] IncidentAssessment incident)
        {
            IncidentAssessment _incidentAssessment;
            int _idConfiguration;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                else if (incident == null)
                    return BadRequest("");

                // Busca valores de Claims carregadas na autenticação 
                var user = ((ClaimsPrincipal)HttpContext.Current.User);
                string idConfiguration = user.Claims.First(a => a.Type == ClaimTypes.Sid).Value;
                string strConfigName = user.Claims.First(a => a.Type == ClaimTypes.Name).Value;
                int.TryParse(idConfiguration, out _idConfiguration);

                // Carrega a Configuration associada ao Incident
                incident.Configuration = new Configuration()
                {
                    Id = _idConfiguration,
                    Name = strConfigName
                };

                // Invoka método de Inserção de Incident
                _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.Create(incident));

                // Valida se foi retonada chave de criação do Incident
                if (string.IsNullOrEmpty(_incidentAssessment.Key))
                    return StatusCode(HttpStatusCode.InternalServerError);

                return Ok(_incidentAssessment);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/IncidentsAssessment/Update/{key}")]
        public async Task<IHttpActionResult> Put([FromUri, Required(AllowEmptyStrings = false)] string key, [FromBody] IncidentAssessment incident)
        {
            IncidentAssessment _incidentAssessment;
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                else if (incident == null)
                    return BadRequest("");

                // Busca o Incidente pela chave informada
                _incidentAssessment = this._incidentAssessmentService.FindByKey(key);

                //Copiar as propriedades
                _incidentAssessment.Copy(incident);

                // Verifica se o Incidente informado está cadastrado
                if (_incidentAssessment == null)
                    return NotFound();

                // Atualiza Placa/Prestador do Incidente
                _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.Update(_incidentAssessment));

                return Ok(_incidentAssessment);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/IncidentsAssessment/FinalizeIncident/{key}")]
        public async Task<IHttpActionResult> FinalizeIncidentAssessment([FromUri, Required(AllowEmptyStrings = false)] string key)
        {
            IncidentAssessment _incidentAssessment;
            try
            {
                // Busca o Incidente pela chave informada
                _incidentAssessment = this._incidentAssessmentService.FindByKey(key);

                // Verifica se o Incidente informado está cadastrado
                if (_incidentAssessment == null)
                    return NotFound();

                // Atualiza o Tipo do Incidente ( Perda Total ou Recuperável pela Oficina ) pela pontuação atingida
                _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.Finalize(_incidentAssessment));

                return Ok(_incidentAssessment);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
