using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface IConfigurationRepository : IBaseRepository
    {
        Configuration Find(int id);

        Configuration FindByAuthenticatedCompany(string login, string password);
    }
}
