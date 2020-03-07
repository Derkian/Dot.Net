using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class AssessmentAdditionalService
    {
        public int IdAssessment { get; set; }

        public Assessment Assessment { get; set; }

        public int IdAssessmentAdditionalService { get; set; }

        public string Description { get; set; }

        public double Value { get; set; }
    }
}
