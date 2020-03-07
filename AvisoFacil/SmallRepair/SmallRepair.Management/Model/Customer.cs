using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class Customer
    {
        public int IdCustomer { get; set; }

        public string Name { get; set; }

        public string ClaimId { get; set; }

        public virtual IList<ServiceValue> ServiceValues { get; set; }

        public virtual IList<Assessment> Assessments { get; set; }

        public virtual IList<AdditionalService> AdditionalServices { get; set; }

        public virtual IList<User> Users { get; set; }
    }
}
