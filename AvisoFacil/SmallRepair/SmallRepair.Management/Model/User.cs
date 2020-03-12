using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class User
    {
        public int IdUser { get; set; }

        public Customer Customer { get; set; }

        public string IdCustomer { get; set; }

        public string ClaimId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
