using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TotalLoss.API.Attributes;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    [CustomAuthorize(TypeCompany.InsuranceCompany)]
    public class TowingCompanyController : ApiController
    {
        #region | Objects

        private readonly TowingCompanyService _towingCompanyService;
        private readonly LoginService _loginService;

        #endregion

        #region | Constructor 

        public TowingCompanyController(TowingCompanyService towingCompanyService, LoginService loginService)
        {
            _towingCompanyService = towingCompanyService;
            _loginService = loginService;
        }

        #endregion

        #region | Actions

        [HttpGet]
        [Route("api/TowingCompanies/GetTowingCompanies")]
        public async Task<IHttpActionResult> GetTowingCompanies()
        {
            IList<TowingCompany> _listTowingCompany = null;
            Company _insuranceCompany = null;

            // Carrega a configuração da Seguradora associada ao Token
            _insuranceCompany = Util.Helper.GetConfiguration();
            
            // Busca todas Empresas de Guinchos por Seguradora informada
            _listTowingCompany = await Task.Run(() => _towingCompanyService.ListByInsuranceCompany(_insuranceCompany));
            
            //retorna as companhias
            return Ok(_listTowingCompany);
        }

        [HttpPost]
        [Route("api/TowingCompanies/Create")]
        public async Task<IHttpActionResult> Post([FromBody] TowingCompany towingCompany)
        {
            TowingCompany _towingCompany = null;
            Company _insuranceCompany = null;
            User _user = null;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (towingCompany == null)
                return BadRequest();

            // Carrega a configuração da Seguradora associada ao Token
            _insuranceCompany = Util.Helper.GetConfiguration();
            
            // Busca Empresa de Guincho com pelo e-mail informado
            _towingCompany = this._towingCompanyService.GetTowingCompany(towingCompany);

            // Busca Usuário cadastrado pelo Login (e-mail) informado
            _user = this._loginService.GetUserByLogin(new User() { Login = towingCompany.Email });

            // Valida se já existe Empresa de Guincho com o -email informado
            if (_towingCompany != null || _user != null)
                return BadRequest("Endereço da conta E-mail informada já existe"); 

            // Atribui o Tipo de Companhia para: Empresa de Guincho  
            towingCompany.TypeCompany = TypeCompany.TowingServiceCompany;

            // Invoka método de inserção da Empresa de Guincho
            _towingCompany = await Task.Run(() => this._towingCompanyService.Create(_insuranceCompany, towingCompany));

            return Ok(_towingCompany);
        }

        [HttpPut]
        [Route("api/TowingCompanies/Update/{idTowingCompany}")]
        public async Task<IHttpActionResult> Put([FromUri] int idTowingCompany, [FromBody] TowingCompany towingCompany)
        {
            TowingCompany _towingCompany = null;
            Company _insuranceCompany = null;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (towingCompany == null)
                return BadRequest();

            // Carrega a configuração da Seguradora associada ao Token
            _insuranceCompany = Util.Helper.GetConfiguration();
            
            // Busca Empresa de Guincho por relacionamento com a Seguradora do Token
            _towingCompany = await Task.Run(() => _towingCompanyService.ListByInsuranceCompany(_insuranceCompany)
                                                                       .FirstOrDefault(c => c.Id == idTowingCompany));

            // Verifica se Empresa de Guincho informada é relacionada com Seguradora do Token
            if (_towingCompany == null)
                return NotFound();

            // Copiar as propriedades
            _towingCompany.Copy(towingCompany);

            // Atualiza dados da Empresa de Guincho
            _towingCompany = await Task.Run(() => this._towingCompanyService.Update(_towingCompany));

            return Ok(_towingCompany);
        }
        
        #endregion
    } 
}
