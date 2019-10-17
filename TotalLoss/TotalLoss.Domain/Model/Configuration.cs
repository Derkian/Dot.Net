using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Script.Serialization;

namespace TotalLoss.Domain.Model
{
    public class Configuration
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CNPJ { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        [JsonIgnore]
        public byte[] Image { get; set; }

        public string Logo
        {
            get
            {
                if (this.Image.Length > 0)
                    return Convert.ToBase64String(this.Image);
                else
                    return "";

            }

        }

        [JsonIgnore]
        public string Login { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public int LimitTotalLoss { get; set; } = 0;
    }
}
