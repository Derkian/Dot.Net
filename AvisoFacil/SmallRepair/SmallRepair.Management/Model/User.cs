using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class User
    {
        public string IdUser { get; set; }

        public Company Company { get; set; }

        public string IdCompany { get; set; }

        public string ClaimId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
