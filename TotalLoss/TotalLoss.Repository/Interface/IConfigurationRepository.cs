using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface IConfigurationRepository : IBaseRepository
    {
        InsuranceCompany Find(Company company);
        InsuranceCompany FindByTowingCompany(Company company);
    }
}
