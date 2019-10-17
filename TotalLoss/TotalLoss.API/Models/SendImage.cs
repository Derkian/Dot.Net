using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TotalLoss.API.Models
{    
    public class SendImage
    {   
        [Required]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set;  }
    }
}