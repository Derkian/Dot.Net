using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class ServiceValue
    {
        public int IdServiceValue { get; set; }

        public string IdCompany { get; set; }

        public Company Company { get; set; }

        public EnmServiceType ServiceType { get; set; }

        public double Value { get; set; }
    }
}
