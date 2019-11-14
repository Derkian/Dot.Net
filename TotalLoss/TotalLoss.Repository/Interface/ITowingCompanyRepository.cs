using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface ITowingCompanyRepository: IBaseRepository
    {
        void Create(TowingCompany towingCompany);

        TowingCompany Find(TowingCompany towingCompany);

        IList<TowingCompany> ListByInsuranceCompany(Company insuranceCompany);

        void Update(TowingCompany towingCompany);
    }
}
