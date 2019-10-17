using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class IncidentAssessmentImage
    {   
        public string Name { get; set; }                
        public string MimeType { get; set; }
        public byte[] Image { get; set; }        
    }
}
