using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface ISalvageCompanyRepository : IBaseRepository
    {
        IList<SalvageCompany> ListSalvageByCompany(Configuration company);

        IList<Location> ListSalvageLocation(SalvageCompany salvage);
    }
}
