using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TotalLoss.Domain.Model
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public byte[] Image { get; set; }

        public string FileBase64
        {
            get
            {
                if (Image.Length > 0)
                    return Convert.ToBase64String(this.Image);
                else
                    return "";                
            }
        }

        [JsonIgnore]
        public int Point { get; set; }

        public IList<Question> Questions { get; set; } = new List<Question>();
    }
}
