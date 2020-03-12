using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Model
{
    public class ClienteViewModel
    {
        public string Id { get; set; }

        public string Nome { get; set; }

        public IList<ValorServicoViewModel> ValorServico { get; set; }
    }

    public class ValorServicoViewModel
    {
        public EnmServiceType ServiceType { get; set; }

        public string Description { get; set; }

        public double Value { get; set; }
    }

    public class ServicoAdicionalModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public double Value { get; set; }
    }
}
