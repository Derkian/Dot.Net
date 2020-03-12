using SmallRepair.Management.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallRepair.Api.Model
{
    public class PartViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public EnmMalfunctionType MalfunctionType { get; set; }

        [Required]
        public EnmIntensityType IntensityType { get; set; }

        [Required]
        public double UnitaryValue { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class ManualPartViewModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public double UnitaryValue { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
