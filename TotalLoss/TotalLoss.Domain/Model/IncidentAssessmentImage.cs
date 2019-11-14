using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class IncidentAssessmentImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Base64Image
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
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    if (!string.IsNullOrEmpty(value))
                        this.Image = Convert.FromBase64String(value);
                }
                catch
                {
                }
            }
        }
        public byte[] Image { get; set; }
        [JsonIgnore]
        public byte[] Thumbnail { get; set; }
        public string Base64Thumbnail
        {
            get
            {
                try
                {
                    if (this.Thumbnail != null && this.Thumbnail.Length > 0)
                        return Convert.ToBase64String(this.Thumbnail);
                    else
                        return "";
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
