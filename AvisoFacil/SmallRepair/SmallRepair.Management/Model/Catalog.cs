using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class Catalog
    {
        public int IdCatalog { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public virtual IList<BaremoTime> BaremoTimes { get; set; }
    }
}
