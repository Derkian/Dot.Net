using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class Baremo
    {
        public int IdBaremo { get; set; }

        public EnmMalfunctionType MalfunctionType { get; set; }

        public EnmIntensityType IntensityType { get; set; }

        public virtual IList<BaremoTime> BaremoTimes { get; set; }
    }
}
