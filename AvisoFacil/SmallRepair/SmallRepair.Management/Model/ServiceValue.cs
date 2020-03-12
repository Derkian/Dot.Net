using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class ServiceValue
    {
        public int IdServiceValue { get; set; }

        public string IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public EnmServiceType ServiceType { get; set; }

        public double Value { get; set; }
    }
}
