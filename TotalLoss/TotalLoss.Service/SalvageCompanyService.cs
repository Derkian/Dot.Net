using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;
using TotalLoss.Repository.Interface;

namespace TotalLoss.Service
{
    public class SalvageCompanyService
    {
        private IWorkRepository _workrepository;
        public SalvageCompanyService(IWorkRepository workRepository)
        {
            this._workrepository = workRepository;
        }

        public IList<SalvageCompany> ListSalvageByCompany(Company company)
        {
            IList<SalvageCompany> listSalvage = null;

            try
            {
                listSalvage = this._workrepository.SalvageCompanyRepository.ListSalvageByCompany(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listSalvage;
        }

        public IList<Location> ListSalvageLocation(SalvageCompany salvage)
        {

            IList<Location> listSalvageLocation = null;

            try
            {
                listSalvageLocation = this._workrepository.SalvageCompanyRepository.ListSalvageLocation(salvage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listSalvageLocation;

        }
    }
}
