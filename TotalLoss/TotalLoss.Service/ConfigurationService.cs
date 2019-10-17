using System;

using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class ConfigurationService
    {
        private IConfigurationRepository _configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            this._configurationRepository = configurationRepository;
        }

        /// <summary>
        /// Busca a Configuração pertencente aos dados da companhia informada
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Configuration GetAuthenticatedCompany(string login, string password)
        {
            Configuration configuration = null;
                                    
            try
            {
                configuration = _configurationRepository.FindByAuthenticatedCompany(login, password);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return configuration;
        }
    }
}
