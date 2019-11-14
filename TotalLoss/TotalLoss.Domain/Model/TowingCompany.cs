using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class TowingCompany : Company
    {
        public string Description { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "E-mail informado está com formato inválido")]
        public string Email { get; set; }

        [JsonIgnore]
        public int IdInsuranceCompany { get; set; }

        public bool Enable { get; set; } = true;

        public void Copy(TowingCompany towingCompany)
        {
            var configuration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TowingCompany, TowingCompany>()
                   .ForMember(d => d.Id, o => o.Ignore())
                   .ForMember(d => d.IdInsuranceCompany, o => o.Ignore())
                   .ForMember(d => d.TypeCompany, o => o.Ignore())
                   .ForAllMembers(cond => cond.Condition((src, dest, srcMember) => srcMember != null));
            });

            var mapper = configuration.CreateMapper();

            mapper.Map(towingCompany, this);
        }
    }
}
