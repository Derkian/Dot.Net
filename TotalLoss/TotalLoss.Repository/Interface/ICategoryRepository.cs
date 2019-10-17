using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface ICategoryRepository : IBaseRepository
    {
        IList<Category> ListByCompany(Configuration configuration);
    }

}
