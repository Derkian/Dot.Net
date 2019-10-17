using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class LocationDetail
    {
        public int Id { get; set; }

        public string Street { get; set; }

        public string District { get; set; }

        public string ZipCode { get; set; }

        public string Email { get; set; }

        public string Fone { get; set; }

        public string Fax { get; set; }

        public string Manager { get; set; }
    }
}
