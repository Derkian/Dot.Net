using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class AssessmentVersion
    {
        public int IdAssessmentVersion { get; set; }

        public int IdAssessment { get; set; }

        public int Version { get; set; }

        public string AssessmentData { get; set; }

        public EnmAssessmentVersion Type { get; set; }

        public double Total { get; set; }

        public string IdUser { get; set; }

        public string Email { get; set; }

        public DateTime ChangeDate { get; set; }
    }

    public enum EnmAssessmentVersion
    {
        AssessmentComplete = 0,
        NewVersion = 1
    }
}
