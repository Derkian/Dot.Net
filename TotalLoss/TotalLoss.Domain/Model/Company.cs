using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TotalLoss.Domain.Attributes;
using TotalLoss.Domain.Enums;

namespace TotalLoss.Domain.Model
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [CnpjValidation]
        public string CNPJ { get; set; }

        public TypeCompany TypeCompany { get; set; }
    }
}
