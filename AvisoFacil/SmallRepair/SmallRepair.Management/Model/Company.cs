﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class Company
    {
        public string IdCompany { get; set; }

        public string Name { get; set; }

        public virtual IList<ServiceValue> ServiceValues { get; set; }

        public virtual IList<Assessment> Assessments { get; set; }

        public virtual IList<AdditionalService> AdditionalServices { get; set; }

        public virtual IList<User> Users { get; set; }        

        public virtual IList<CompanyReportTemplate> ReportTemplates { get; set; }
    }
}