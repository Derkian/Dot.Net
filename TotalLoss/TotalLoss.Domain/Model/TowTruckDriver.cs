using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class TowTruckDriver
    {
        public int? Id { get; set; }

        [JsonIgnore]
        public int IdTowingCompany { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[0-9]{12,14}", ErrorMessage = "O Telefone deve possuir 12 dígitos, no formato DDIDDDFONE EX: 551199990101")]
        public string Mobile { get; set; }

        public bool Enable { get; set; } = true;

        public void Copy(TowTruckDriver towTruckDriver)
        {
            var configuration = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TowTruckDriver, TowTruckDriver>()
                   .ForMember(d => d.Id, o => o.Ignore())
                   .ForMember(d => d.IdTowingCompany, o => o.Ignore())                   
                   .ForAllMembers(cond => cond.Condition((src, dest, srcMember) => srcMember != null));
            });

            var mapper = configuration.CreateMapper();

            mapper.Map(towTruckDriver, this);
        }
    }
}
