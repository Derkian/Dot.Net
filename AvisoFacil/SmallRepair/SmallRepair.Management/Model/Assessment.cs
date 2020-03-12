using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class Assessment
    {
        public int IdAssessment { get; set; }

        public Customer Customer { get; set; }

        public string IdCustomer { get; set; }

        //Placa
        public string Plate { get; set; }

        //Modelo
        public string Model { get; set; }

        //Quilometragem
        public string Mileage { get; set; }

        public string CustomerName { get; set; }

        //tipo de carroceria
        public string BodyType { get; set; }

        public virtual IList<AssessmentServiceValue> AssessmentServicesValues { get; set; }

        public virtual IList<Part> Parts { get; set; }

        public virtual IList<AssessmentAdditionalService> AssessmentAdditionalServices { get; set; }

        public double Total { get; set; }
    }
}
