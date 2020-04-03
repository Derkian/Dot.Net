using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Model
{
    public class AssessmentViewModel
    {
        public int Id { get; set; }

        public string Plate { get; set; }

        public EnmAssessmentState State { get; set; }

        public string Model { get; set; }

        public string Mileage { get; set; }

        public string CustomerName { get; set; }

        public string BodyType { get; set; }

        public IList<ServiceValueViewModel> ServiceValues { get; set; }        
    }
}
