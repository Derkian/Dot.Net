using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Repository.Interface
{
    public interface IBaseRepository
    {
        IDbTransaction Transacao { set; }
    }
}
