using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class Location
    {
        public int Id { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public IList<LocationDetail> Detail { get; set; } = new List<LocationDetail>();
    }
}
