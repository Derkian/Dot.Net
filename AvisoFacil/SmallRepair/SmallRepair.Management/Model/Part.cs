using SmallRepair.Management.Enum;
using System.Collections.Generic;

namespace SmallRepair.Management.Model
{
    public class Part
    {
        public int IdPart { get; set; }

        public int IdAssessment { get; set; }

        public Assessment Assessment { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public EnmMalfunctionType MalfunctionType { get; set; }

        public EnmIntensityType IntensityType { get; set; }

        public double UnitaryValue { get; set; }

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        public double TotalService { get; set; }

        public double TotalMaterial { get; set; }

        public double Total { get; set; }

        public virtual IList<Service> Services { get; set; }
    }
}
