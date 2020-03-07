using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class BaremoTime
    {
        public int IdBaremo { get; set; }

        public Baremo Baremo { get; set; }

        public int IdCatalog { get; set; }

        public Catalog Catalog { get; set; }

        public EnmServiceType ServiceType { get; set; }

        public double ServiceTime { get; set; }

        public double MaterialValue { get; set; }
    }
}
