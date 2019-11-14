using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface IHistoryRepository : IBaseRepository
    {
        bool AddHistory(History history);
    }
}
