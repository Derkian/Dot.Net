using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class SalvageCompany
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RegistrationNumber { get; set; }

        public IList<Location> Location { get; set; } = new List<Location>();
    }
}
