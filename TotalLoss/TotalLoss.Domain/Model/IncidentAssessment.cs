using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalLoss.Domain.Enums;

namespace TotalLoss.Domain.Model
{
    public class IncidentAssessment
    {
        public IncidentAssessment() { }

        public IncidentAssessment(string key)
        {
            this.Key = key;
        }

        public int Id { get; set; }

        public Configuration Configuration { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        public string ClaimNumber { get; set; }

        public string InsuredName { get; set; }

        public string InsuredFone { get; set; }

        public string Provider { get; set; }
        
        public string WorkProvider { get; set; }

        [Required]        
        [RegularExpression("^[0-9]{12,14}", ErrorMessage = "O Telefone deve possuir 12 dígitos, no formato DDIDDDFONE EX: 555199990101")]
        public string WorkProviderFone { get; set; }

        public string Key
        {
            get
            {
                var value = Encoding.UTF8.GetBytes(this.Id.ToString());
                return Convert.ToBase64String(value);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        var result = Convert.FromBase64String(value);
                        var resultText = Encoding.UTF8.GetString(result);
                        int resultInt = 0;

                        if (Int32.TryParse(resultText, out resultInt))
                            this.Id = resultInt;
                    }
                    catch (Exception ex)
                    {
                        this.Id = 0;
                    }                    
                }
            }
        }
        
        public ShortMessageCode ShortMessageCode { get; set; } = ShortMessageCode.NotSend;

        public TypeIncidentAssessment Type { get; set; } = TypeIncidentAssessment.None;

        public StatusIncidentAssessment Status { get; set; } = StatusIncidentAssessment.Created;

        public int TotalPoint { get; set; }

        public IList<Category> Categories { get; set; } = new List<Category>();

        public void Copy(IncidentAssessment inicident)
        {
            this.LicensePlate = string.IsNullOrEmpty(inicident.LicensePlate) ? this.LicensePlate : inicident.LicensePlate;
            this.ClaimNumber = string.IsNullOrEmpty(inicident.ClaimNumber) ? this.ClaimNumber : inicident.ClaimNumber;
            this.InsuredName = string.IsNullOrEmpty(inicident.InsuredName) ? this.InsuredName : inicident.InsuredName;
            this.InsuredFone = string.IsNullOrEmpty(inicident.InsuredFone) ? this.InsuredFone : inicident.InsuredFone;
            this.Provider = string.IsNullOrEmpty(inicident.Provider) ? this.Provider : inicident.Provider;
            this.WorkProvider = string.IsNullOrEmpty(inicident.WorkProvider) ? this.WorkProvider : inicident.WorkProvider;            
        }
    }
}
