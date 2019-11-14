using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class SalvageCompanyController : ApiController
    {
        #region Object
        private SalvageCompanyService _service;
        #endregion

        #region Constructor
        public SalvageCompanyController(SalvageCompanyService service)
        {
            this._service = service;
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("api/SalvagesCompanies/GetSalvagesCompanies/{idInsuranceCompany}")]
        public async Task<IHttpActionResult> GetSalvagesByCompany(int idInsuranceCompany)
        {
            IList<SalvageCompany> listSalvage = null;
            Domain.Model.Company request = null;

            // Cria entidade Configuration
            request = new Domain.Model.Company() { Id = idInsuranceCompany };

            // Busca empresas de Pátios cadastradas por Segurada informada
            listSalvage = await Task.Run(() => this._service.ListSalvageByCompany(request));

            // Verifica se existem empresas de Pátios associadas à Seguradora informada
            if (listSalvage == null || listSalvage.Count == 0)
                return NotFound();

            // Retorna response com todas Categorias por Campanhia
            return Ok(listSalvage);
        }

        [HttpGet]
        [Route("api/SalvagesCompanies/GetSalvageCompanyLocations/{idSalvageCompany}")]
        public async Task<IHttpActionResult> GetSalvageCompanyLocation(int idSalvageCompany)
        {
            IList<Location> listLocation = null;
            Domain.Model.SalvageCompany request = null;

            // Cria entidade SalvageCompany
            request = new Domain.Model.SalvageCompany() { Id = idSalvageCompany };

            // Busca todas Localizações da empresa de Pátio informada
            listLocation = await Task.Run(() => this._service.ListSalvageLocation(request));

            // Verifica se existem localizações para a empresa de Pátio informada
            if (listLocation == null || listLocation.Count == 0)
                return NotFound();

            return Ok(listLocation);
        }
        #endregion
    }
}
