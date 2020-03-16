using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class CompanyReportTemplate
    {
        public string IdCompany { get; set; }

        public Company Company{ get; set; }

        public string Code { get; set; }

        public string Template { get; set; }
    }
}
