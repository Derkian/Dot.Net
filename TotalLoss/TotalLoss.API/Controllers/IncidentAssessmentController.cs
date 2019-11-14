using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
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
using System.Web.Http.ModelBinding;
using TotalLoss.API.Attributes;
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
        private ConfigurationService _configurationService;

        #endregion

        #region | Constructor 

        /// <summary>
        /// Construtor da Classe
        /// </summary>
        /// <param name="incidentAssessmentService"></param>
        /// <param name="configurationService"></param>
        public IncidentAssessmentController(IncidentAssessmentService incidentAssessmentService, ConfigurationService configurationService)
        {
            this._incidentAssessmentService = incidentAssessmentService;
            this._configurationService = configurationService;

            //adicionar o usuário que realizará alterações
            this._incidentAssessmentService.LogHistoryUser = Util.Helper.GetUser();
        }

        #endregion

        #region | Actions 

        /// <summary>
        /// Adicionar respostas ao Incident
        /// </summary>
        /// <param name="key"></param>
        /// <param name="questions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/IncidentsAssessment/Answer/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessment))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ModelError))]
        public async Task<IHttpActionResult> AddIncidentAssessmentAnswer([FromUri, Required(AllowEmptyStrings = false)] string key,
                                                                         [FromBody] List<Question> questions)
        {
            IncidentAssessment _incidentAssessment;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (questions == null)
                return BadRequest("");

            _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.AddAnswers(key, questions));

            return Ok(_incidentAssessment);
        }

        /// <summary>
        /// Adicionar Imagens ao Incident
        /// </summary>
        /// <param name="key"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/IncidentsAssessment/Image/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        public async Task<IHttpActionResult> AddIncidentAssessmentImage([FromUri, Required(AllowEmptyStrings = false)] string key, [FromBody] SendImage image)
        {
            byte[] ImageBinaryContent;


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
                Image = ImageBinaryContent
            };

            bool result = await Task.Run(() => this._incidentAssessmentService.AddImage(key, incidentImage));

            return Ok(result);
        }

        /// <summary>
        /// Apaga a imagem do incident
        /// </summary>
        /// <param name="key"></param>
        /// <param name="idImage"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/IncidentsAssessment/Image/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        public async Task<IHttpActionResult> DeleteIncidentAssessmentImage([FromUri, Required(AllowEmptyStrings = false)] string key, int idImage)
        {
            //validação
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key é obrigatório");
            else if (idImage < 0)
                return BadRequest("idImage é obrigatório");

            //apaga a imagem
            bool result = await Task.Run(() => this._incidentAssessmentService.DeleteImage(key, idImage));

            return Ok(result);
        }

        /// <summary>
        /// Cria um novo incident
        /// </summary>
        /// <param name="incident"></param>
        /// <returns></returns>
        [CustomAuthorize(TypeCompany.InsuranceCompany)]
        [HttpPost]
        [Route("api/IncidentsAssessment/Create")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessment))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ModelError))]
        public async Task<IHttpActionResult> Post([FromBody] IncidentAssessment incident)
        {
            IncidentAssessment _incidentAssessment;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (incident == null)
                return BadRequest();

            // Carrega a Seguradora associada ao Token
            Company configurationCompany = Util.Helper.GetConfiguration();

            // Invoka método de Inserção de Incident
            _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.Create(incident, configurationCompany));

            // Valida se foi retonada chave de criação do Incident
            if (string.IsNullOrEmpty(_incidentAssessment.Key))
                return StatusCode(HttpStatusCode.InternalServerError);

            return Ok(_incidentAssessment);
        }

        /// <summary>
        /// Envia SMS para o Motorista
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/IncidentsAssessment/SendSms/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessment))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(ModelError))]
        public async Task<IHttpActionResult> SendSms([FromUri, Required(AllowEmptyStrings = false)] string key)
        {
            IncidentAssessment _incidentAssessment;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (key == null)
                return BadRequest();

            // Carrega a Seguradora associada ao Token
            Company configurationCompany = Util.Helper.GetConfiguration();

            // Invoka método de Envio de SMS
            _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.SendSMS(key, configurationCompany));

            if (_incidentAssessment == null)
                return NotFound();

            //retorna o incident
            return Ok(_incidentAssessment);
        }

        /// <summary>
        /// Carrega o Incident
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IncidentsAssessment/GetIncident/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessment))]

        public async Task<IHttpActionResult> GetIncidentAssessmentByKey([FromUri, Required(AllowEmptyStrings = false)] string key)
        {
            IncidentAssessment _incidentAssessment = null;
            InsuranceCompany _configurationCompany = null;

            // Busca Incidente por ID criptografado
            _incidentAssessment = await Task.Run(() => _incidentAssessmentService.FindByKey(key));

            if (_incidentAssessment == null)
                return NotFound();

            // Carrega a Seguradora associada ao Incidente
            Company company = new Company { Id = _incidentAssessment.IdInsuranceCompany, TypeCompany = TypeCompany.InsuranceCompany };

            // Busca Configuration Company
            _configurationCompany = await Task.Run(() => _configurationService.GetConfiguration(company));

            // Verifica se existe Incidente por ID informado
            if (_incidentAssessment == null)
                return NotFound();

            // Retorna response com todas Categorias por Campanhia
            return Ok(new { Company = _configurationCompany, Incident = _incidentAssessment });
        }

        /// <summary>
        /// Lista os Incidents
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IncidentsAssessment/List")]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<Pagination<IncidentAssessment>>))]
        public async Task<IHttpActionResult> ListIncident(int pageNumber = 1, int pageSize = 10, StatusIncidentAssessment? status = null)
        {
            //recupera a empresa 
            Company company = Util.Helper.GetConfiguration();

            //se a objeto for nulo, recupera a primeira página            
            var page = new Pagination<IncidentAssessment>() { PageNumber = pageNumber, PageSize = pageSize };

            //executa a consulta
            page = await Task.Run(() => this._incidentAssessmentService.Paginate(company, page, status));

            //devolve a lista de Inicidentes
            return Ok(page);
        }

        /// <summary>
        /// Lista as imagens do Incident
        /// </summary>
        /// <param name="key"></param>
        /// <param name="imageNumber"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IncidentsAssessment/Image/List/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<Pagination<IncidentAssessmentImage>>))]
        public async Task<IHttpActionResult> ListIncidentImage(string key, int pageNumber = 1, int pageSize = 10)
        {
            //se a objeto for nulo, recupera a primeira página            
            var page = new Pagination<IncidentAssessmentImage>() { PageNumber = pageNumber, PageSize = pageSize };

            //executa a consulta
            page = await Task.Run(() => this._incidentAssessmentService.PaginateImage(key, page));

            //devolve a lista de Inicidentes
            return Ok(page);
        }

        /// <summary>
        /// Retorna o conteudo da imagem
        /// </summary>
        /// <param name="key"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("api/IncidentsAssessment/Image/Get/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessmentImage))]
        public async Task<IHttpActionResult> GetIncidentImage(string key, int idImage)
        {
            //executa a consulta
            var image = await Task.Run(() => this._incidentAssessmentService.GetImage(key, idImage));

            //devolve a lista de Inicidentes
            return Ok(image);
        }

        /// <summary>
        /// Retorna o Thumbnail da Imagem
        /// </summary>
        /// <param name="key"></param>
        /// <param name="idImage"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IncidentsAssessment/Image/Thumbnail/Get/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessmentImage))]
        public async Task<IHttpActionResult> GetIncidentThumbnailImage(string key, int idImage)
        {
            //executa a consulta
            var image = await Task.Run(() => this._incidentAssessmentService.GetThumbnailImage(key, idImage));

            //devolve a lista de Inicidentes
            return Ok(image);
        }

        /// <summary>
        /// Atualiza os dados do incident
        /// </summary>
        /// <param name="key"></param>
        /// <param name="incident"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/IncidentsAssessment/Update/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessment))]
        public async Task<IHttpActionResult> Patch([FromUri, Required(AllowEmptyStrings = false)] string key, [FromBody] IncidentAssessment incident)
        {
            IncidentAssessment _incidentAssessment;

            if (incident == null)
                return BadRequest("");

            // Atualiza Placa/Prestador do Incidente
            _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.Update(key, incident));

            return Ok(_incidentAssessment);
        }

        /// <summary>
        /// Finaliza o processo
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/IncidentsAssessment/FinalizeIncident/{key}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IncidentAssessment))]
        public async Task<IHttpActionResult> FinalizeIncidentAssessment([FromUri, Required(AllowEmptyStrings = false)] string key)
        {
            IncidentAssessment _incidentAssessment;

            // Atualiza o Tipo do Incidente ( Perda Total ou Recuperável pela Oficina ) pela pontuação atingida
            _incidentAssessment = await Task.Run(() => this._incidentAssessmentService.Finalize(key));

            // Verifica se o Incidente informado está cadastrado
            if (_incidentAssessment == null)
                return NotFound();

            return Ok(_incidentAssessment);
        }

        #endregion
    }
}
