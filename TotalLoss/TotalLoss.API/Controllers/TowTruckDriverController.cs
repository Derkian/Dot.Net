using System;
using System.Collections.Generic;
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
    [CustomAuthorize(TypeCompany.TowingServiceCompany)]
    public class TowTruckDriverController : ApiController
    {
        #region | Objects

        private readonly TowTruckDriverService _towTruckDriverService;

        #endregion

        #region | Constructor 

        public TowTruckDriverController(TowTruckDriverService towTruckDriverService)
        {
            _towTruckDriverService = towTruckDriverService;
        }

        #endregion

        #region | Actions

        [HttpGet]
        [Route("api/TowTruckDrivers/GetTowTruckDrivers")]
        public async Task<IHttpActionResult> GetTowTruckDrivers()
        {
            IList<TowTruckDriver> _listTowTruckDriver = null;
            Company _towingCompany = null;

            // Carrega a configuração da Empresa de Guincho associada ao Token
            _towingCompany = Util.Helper.GetConfiguration();
            
            // Busca todos os Motoristas por Empresa de Guincho 
            _listTowTruckDriver = await Task.Run(() => _towTruckDriverService.ListByTowingCompany(_towingCompany));

            //retorna os motoristas            
            return Ok(_listTowTruckDriver);
        }

        [HttpPost]
        [Route("api/TowTruckDrivers/Create")]
        public async Task<IHttpActionResult> Post([FromBody] TowTruckDriver towTruckDriver)
        {
            TowTruckDriver _towTruckDriver = null;
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (towTruckDriver == null)
                return BadRequest();
            
            // Recupera a companhia de reboque
            var company = Util.Helper.GetConfiguration();     

            // Atribui o ID da companhia de reboque ao motorista
            towTruckDriver.IdTowingCompany = company.Id;

            // Invoka método de inserção do Motorista de Guincho
            _towTruckDriver = await Task.Run(() => this._towTruckDriverService.Create(towTruckDriver));

            return Ok(_towTruckDriver);
        }

        [HttpPut]
        [Route("api/TowTruckDrivers/Update/{idTowTruckDriver}")]
        public async Task<IHttpActionResult> Put([FromUri] int idTowTruckDriver, [FromBody] TowTruckDriver towTruckDriver)
        {
            TowTruckDriver _towTruckDriver = null;
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (towTruckDriver == null)
                return BadRequest();

            // Recupera a Companhia de Reboque
            var company = Util.Helper.GetConfiguration();
            
            // Busca cadastro do Motorista de Guincho informado
            _towTruckDriver = await Task.Run(() => _towTruckDriverService.Find(idTowTruckDriver));

            // Verifica se Motorista de Guincho informado está cadastrado
            if (_towTruckDriver == null && _towTruckDriver.IdTowingCompany != company.Id)
                return NotFound();

            // Copiar as propriedades
            _towTruckDriver.Copy(towTruckDriver);

            // Atualiza dados do Motorista de Guincho 
            _towTruckDriver = await Task.Run(() => this._towTruckDriverService.Update(_towTruckDriver));

            return Ok(_towTruckDriver);
        }

        #endregion
    }
}
