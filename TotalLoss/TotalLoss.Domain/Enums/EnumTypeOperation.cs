using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Enums
{
    public enum TypeOperation
    {
        CreateIncident = 1,
        UpdateIncident = 2,        
        GetIncident = 3,
        AddImage = 4,
        AddAnswear = 5,
        FinalizeIncident = 6,
        SendSms = 7,
        DeleteImage = 8
    }
}
