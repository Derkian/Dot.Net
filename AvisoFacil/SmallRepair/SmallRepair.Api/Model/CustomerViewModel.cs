using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Model
{
    public class CompanyViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IList<ServiceValueViewModel> ServiceValues { get; set; }
    }

    public class ServiceValueViewModel
    {
        public EnmServiceType ServiceType { get; set; }

        public string Description { get; set; }

        public double Value { get; set; }
    }
}
