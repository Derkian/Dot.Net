using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TotalLoss.Domain.Enums;

namespace TotalLoss.Domain.Model
{
    public class IncidentAssessment
    {
        #region | Constructor 
        public IncidentAssessment() { }

        public IncidentAssessment(string key)
        {
            this.Key = key;
        }

        #endregion

        #region | Properties 

        public int Id { get; set; }

        public int IdInsuranceCompany { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        public string ClaimNumber { get; set; }

        public string InsuredName { get; set; }

        public string InsuredPhone { get; set; }

        public Company InsuranceCompany { get; set; }

        public TowingCompany TowingCompany { get; set; }

        public TowTruckDriver TowTruckDriver { get; set; }

        public string TowTruckDriverName { get; set; }

        [RegularExpression("^[0-9]{12,14}", ErrorMessage = "O Telefone deve possuir 12 dígitos, no formato DDIDDDFONE EX: 551199990101")]
        public string TowTruckDriverMobile { get; set; }

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
                    catch
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

        public IList<Question> Answers { get; set; } = new List<Question>();

        public DateTime CreateDate { get; set; }

        #endregion

        #region | Method 

        public void Copy(IncidentAssessment inicident)
        {
            var configuration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IncidentAssessment, IncidentAssessment>()                    
                   .ForMember(d => d.Id, o => o.Ignore())
                   .ForMember(d => d.Status, o => o.Ignore())
                   .ForMember(d => d.Type, o => o.Ignore())
                   .ForMember(d => d.ShortMessageCode, o => o.Ignore())
                   .ForMember(d => d.IdInsuranceCompany, o => o.Ignore())
                   .ForMember(d => d.InsuranceCompany, o => o.Ignore())
                   .ForMember(d => d.Key, o => o.Ignore())
                   .ForMember(d => d.CreateDate, o => o.Ignore())
                   .ForAllMembers(cond => cond.Condition((src, dest, srcMember) => srcMember != null));
            });

            var mapper = configuration.CreateMapper();

            mapper.Map(inicident, this);
        }

        #endregion
    }
}
