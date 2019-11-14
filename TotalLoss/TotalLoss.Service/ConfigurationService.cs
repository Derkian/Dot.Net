using System;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class ConfigurationService
    {
        private IWorkRepository _workRepository;

        public ConfigurationService(IWorkRepository workRepository)
        {
            this._workRepository = workRepository;
        }

        /// <summary>
        ///  Busca configuração da Companhia por ID e Tipo de Companhia
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public InsuranceCompany GetConfiguration(Company company)
        {
            InsuranceCompany configurationCompany = null;

            try
            {
                if (company.TypeCompany == TypeCompany.InsuranceCompany)
                    configurationCompany = this._workRepository.ConfigurationRepository.Find(company);
                else
                    configurationCompany = this._workRepository.ConfigurationRepository.FindByTowingCompany(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return configurationCompany;            
        }
    }
}
