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
        private ISalvageCompanyRepository _repository;
        public SalvageCompanyService(ISalvageCompanyRepository repository)
        {
            this._repository = repository;
        }

        public IList<SalvageCompany> ListSalvageByCompany(Configuration company)
        {
            IList<SalvageCompany> listSalvage = null;

            try
            {
                listSalvage = _repository.ListSalvageByCompany(company);
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
                listSalvageLocation = _repository.ListSalvageLocation(salvage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listSalvageLocation;

        }
    }
}
