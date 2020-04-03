using SmallRepair.Management.Enum;
using SmallRepair.Management.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Business.Model
{
    public class AssessmentReport
    {
        public Assessment Assessment { get; set; }

        public IEnumerable<AssessmentSummary> Summaries { get; set; } = new List<AssessmentSummary>();
    }

    public class AssessmentSummary
    {
        public EnmServiceType? ServiceType { get; set; }

        public string Description { get; set; }

        public double? AmountHours { get; set; }

        public double? ValuePerHour { get; set; }

        public double? TotalMaterial { get; set; }

        public double? TotalService { get; set; }

        public double? Total { get; set; }
    }
}
