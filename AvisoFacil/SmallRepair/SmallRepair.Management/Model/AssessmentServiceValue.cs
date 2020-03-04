using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class AssessmentServiceValue
    {
        public int IdAssessmentServiceValue { get; set; }

        public int IdAssessment { get; set; }

        public Assessment Assessment { get; set; }

        public EnmServiceType ServiceType { get; set; }

        public double Value { get; set; }
    }
}
