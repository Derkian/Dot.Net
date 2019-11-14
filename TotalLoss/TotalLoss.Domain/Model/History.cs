using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;

namespace TotalLoss.Domain.Model
{
    public class History
    {
        //Chame primaria da Historico
        public long Id { get; set; }
        //Operação Realizada INSERT / UPDATE / DELETE / ETC
        public TypeOperation Operation { get; set; }
        //registra o usuário que realizou a ação
        public User User { get; set; }
        //Objeto a ser serializado em JSON
        public IncidentAssessment incidentAssessment { get; set; }
        //Data da ocorrência
        public DateTime Date { get; set; }
        //Objeto serializado em Json
        public string ObjectJson
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this.incidentAssessment);
            }
        }
    }
}
