﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Management.Model
{
    public class AdditionalService
    {
        public int IdAdditionalService { get; set; }

        public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public string Description { get; set; }

        public double Value { get; set; }
    }
}