using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Model;

namespace TotalLoss.Repository.Interface
{
    public interface ITowTruckDriverRepository : IBaseRepository
    {
        void Create(TowTruckDriver towTruckDriver);

        TowTruckDriver Find(int id);

        IList<TowTruckDriver> ListByTowingCompany(Company towingCompany);

        void Update(TowTruckDriver towTruckDriver);

        void Delete(int id);
    }
}
