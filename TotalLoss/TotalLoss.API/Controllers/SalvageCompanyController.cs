using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class SalvageCompanyController : ApiController
    {
        private SalvageCompanyService _service;

        public SalvageCompanyController(SalvageCompanyService service)
        {
            this._service = service;
        }

        [HttpGet]
        [Route("api/SalvagesCompanies/GetSalvagesCompanies/{idInsuranceCompany}")]        
        public async Task<IHttpActionResult> GetSalvagesByCompany(int idInsuranceCompany)
        {
            IList<SalvageCompany> listSalvage = null;
            Domain.Model.Configuration request = null;
            try
            {
                // Cria entidade Configuration
                request = new Domain.Model.Configuration() { Id = idInsuranceCompany };

                // Busca todas Categorias por Companhia informada
                listSalvage = await Task.Run(() => this._service.ListSalvageByCompany(request));

                // Verifica se existem Categorias pela Companhia informada
                if (listSalvage == null)
                    return NotFound();
            }
            catch (System.Exception ex)
            {
                // Retorna Json de Erro interno gerado 
                return InternalServerError(new System.Exception(ex.Message));
            }

            // Retorna response com todas Categorias por Campanhia
            return Ok(listSalvage);
        }

        [HttpGet]
        [Route("api/SalvagesCompanies/GetSalvageCompanyLocations/{idSalvageCompany}")]
        public async Task<IHttpActionResult> GetSalvageCompanyLocation(int idSalvageCompany)
        {
            IList<Location> listSalvage = null;
            Domain.Model.SalvageCompany request = null;
            try
            {
                // Cria entidade Configuration
                request = new Domain.Model.SalvageCompany() { Id = idSalvageCompany };

                // Busca todas Categorias por Companhia informada
                listSalvage = await Task.Run(() => this._service.ListSalvageLocation(request));

                // Verifica se existem Categorias pela Companhia informada
                if (listSalvage == null)
                    return NotFound();
            }
            catch (System.Exception ex)
            {
                // Retorna Json de Erro interno gerado 
                return InternalServerError(new System.Exception(ex.Message));
            }

            // Retorna response com todas Categorias por Campanhia
            return Ok(listSalvage);
        }
    }
}
