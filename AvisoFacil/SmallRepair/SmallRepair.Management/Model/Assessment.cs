using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class Assessment
    {
        public int IdAssessment { get; set; }

        public int Version { get; set; }

        public Company Company { get; set; }

        public string IdCompany { get; set; }

        public EnmAssessmentState State { get; set; }

        //Placa
        public string Plate { get; set; }

        //Modelo
        public string Model { get; set; }

        //Quilometragem
        public string Mileage { get; set; }

        public string CustomerName { get; set; }

        //tipo de carroceria
        public string BodyType { get; set; }

        public virtual IList<AssessmentServiceValue> ServicesValues { get; set; }

        public virtual IList<Part> Parts { get; set; }

        public virtual IList<AssessmentAdditionalService> AdditionalServices { get; set; }

        public double Total { get; set; }

        public DateTime Created { get; set; }

        public User Inspector { get; private set; }

        public void SetInspector(User user)
        {
            Inspector = user;
        }
    }
}
