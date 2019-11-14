using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;
using TotalLoss.Service;

namespace TotalLoss.API.Controllers
{
    public class ConfigurationController : ApiController
    {
        #region | Objects

        private readonly ConfigurationService _configurationService;

        #endregion

        #region | Constructor  

        public ConfigurationController(ConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region | Actions

        [Authorize]
        [HttpGet]
        [Route("api/Configuration/GetConfiguration")]
        public async Task<IHttpActionResult> GetConfiguration()
        {
            Domain.Model.Company _configurationCompany = null;

            // Carrega a configuração da Seguradora associada ao Token
            Company company = Util.Helper.GetConfiguration();

            // Busca configuração da Companhia por usuário
            _configurationCompany = await Task.Run(() => _configurationService.GetConfiguration(company));

            // Verifica se existe Companhia pelo usuário informado
            if (_configurationCompany == null)
                return NotFound();

            return Ok(_configurationCompany);
        }

        #endregion
    }
}
