using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class InsuranceCompany : Company
    {
        public string Description { get; set; }

        [JsonIgnore]
        public byte[] Image { get; set; }

        public string Logo
        {
            get
            {
                try
                {
                    if (this.Image != null && this.Image.Length > 0)
                        return Convert.ToBase64String(this.Image);
                    else
                        return "";
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        [JsonIgnore]
        public int LimitTotalLoss { get; set; } = 0;
    }
}
